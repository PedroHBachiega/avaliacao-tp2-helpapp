using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Infra.Data.Repositories
{
    public class EmployeeEvaluationRepository : IEmployeeEvaluationRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeEvaluationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeEvaluation>> GetEvaluationsAsync()
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeEvaluation>> GetEvaluationsByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeEvaluations
                .Where(e => e.EmployeeId == employeeId)
                .Include(e => e.Employee)
                .ToListAsync();
        }

        public async Task<EmployeeEvaluation> GetByIdAsync(int? id)
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<EmployeeEvaluation> CreateAsync(EmployeeEvaluation evaluation)
        {
            _context.EmployeeEvaluations.Add(evaluation);
            await _context.SaveChangesAsync();
            return evaluation;
        }

        public async Task<EmployeeEvaluation> UpdateAsync(EmployeeEvaluation evaluation)
        {
            _context.EmployeeEvaluations.Update(evaluation);
            await _context.SaveChangesAsync();
            return evaluation;
        }

        public async Task<bool> RemoveAsync(EmployeeEvaluation evaluation)
        {
            _context.EmployeeEvaluations.Remove(evaluation);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}