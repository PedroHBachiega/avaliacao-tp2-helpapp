using StockApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    public interface IEmployeeEvaluationRepository
    {
        Task<IEnumerable<EmployeeEvaluation>> GetEvaluationsAsync();
        Task<IEnumerable<EmployeeEvaluation>> GetEvaluationsByEmployeeIdAsync(int employeeId);
        Task<EmployeeEvaluation> GetByIdAsync(int? id);
        Task<EmployeeEvaluation> CreateAsync(EmployeeEvaluation evaluation);
        Task<EmployeeEvaluation> UpdateAsync(EmployeeEvaluation evaluation);
        Task<bool> RemoveAsync(EmployeeEvaluation evaluation);
    }
}