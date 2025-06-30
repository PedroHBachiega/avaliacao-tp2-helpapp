using StockApp.Domain.Entities;

namespace StockApp.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogChangeAsync(string entityName, int entityId, string action, 
            string oldValues, string newValues, string userId);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string entityName, int? entityId);
    }

}

