using StockApp.Domain.Entities;
using StockApp.Application.Interfaces;
using StockApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace StockApp.Infra.Data.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogChangeAsync(string entityName, int entityId, string action, 
                                       string oldValues, string newValues, string userId)
        {
            var log = new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues,
                NewValues = newValues,
                ChangedBy = userId,
                ChangedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string entityName, int? entityId)
        {
            var query = _context.AuditLogs.Where(a => a.EntityName == entityName);
    
            if (entityId.HasValue)
                query = query.Where(a => a.EntityId == entityId.Value);
        
            return await query.OrderByDescending(a => a.ChangedAt).ToListAsync();
        }

        
    }
}