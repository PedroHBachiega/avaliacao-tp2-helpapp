using StockApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    /// <summary>
    /// Interface para repositório de contratos
    /// </summary>
    public interface IContractRepository
    {
        /// <summary>
        /// Obtém todos os contratos
        /// </summary>
        Task<IEnumerable<Contract>> GetAllAsync();
        
        /// <summary>
        /// Obtém contratos por ID de fornecedor
        /// </summary>
        Task<IEnumerable<Contract>> GetBySupplierIdAsync(int supplierId);
        
        /// <summary>
        /// Obtém contratos por ID de cliente
        /// </summary>
        Task<IEnumerable<Contract>> GetByClientIdAsync(int clientId);
        
        /// <summary>
        /// Obtém contrato por ID
        /// </summary>
        Task<Contract?> GetByIdAsync(int id);
        
        /// <summary>
        /// Cria um novo contrato
        /// </summary>
        Task<Contract> CreateAsync(Contract contract);
        
        /// <summary>
        /// Atualiza um contrato existente
        /// </summary>
        Task<Contract> UpdateAsync(Contract contract);
        
        /// <summary>
        /// Remove um contrato
        /// </summary>
        Task<bool> DeleteAsync(int id);
        
        /// <summary>
        /// Verifica se um fornecedor possui contrato ativo
        /// </summary>
        Task<bool> HasActiveContractAsync(int? supplierId, int? clientId);
        
        /// <summary>
        /// Obtém contratos próximos da expiração
        /// </summary>
        Task<IEnumerable<Contract>> GetContractsNearExpirationAsync(int daysThreshold);
    }
}