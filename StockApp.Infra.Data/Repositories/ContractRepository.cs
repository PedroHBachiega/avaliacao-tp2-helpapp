using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Infra.Data.Repositories
{
    /// <summary>
    /// Repositório para contratos
    /// </summary>
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Supplier)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contract>> GetBySupplierIdAsync(int supplierId)
        {
            return await _context.Contracts
                .Where(c => c.SupplierId == supplierId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<Contract>> GetByClientIdAsync(int clientId)
        {
            return await _context.Contracts
                .Where(c => c.ClientId == clientId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Supplier)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Contract> CreateAsync(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<Contract> UpdateAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contract = await GetByIdAsync(id);
            if (contract == null)
                return false;

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasActiveContractAsync(int? supplierId, int? clientId)
        {
            if (supplierId.HasValue)
            {
                return await _context.Contracts
                    .AnyAsync(c => c.SupplierId == supplierId && c.IsActive && c.EndDate >= DateTime.Now);
            }
            else if (clientId.HasValue)
            {
                return await _context.Contracts
                    .AnyAsync(c => c.ClientId == clientId && c.IsActive && c.EndDate >= DateTime.Now);
            }
            
            return false;
        }
        
        public async Task<IEnumerable<Contract>> GetContractsNearExpirationAsync(int daysThreshold)
        {
            var limitDate = DateTime.Now.AddDays(daysThreshold);
            
            return await _context.Contracts
                .Where(c => c.IsActive && c.EndDate <= limitDate && c.EndDate >= DateTime.Now)
                .OrderBy(c => c.EndDate)
                .ToListAsync();
        }
    }
}