using StockApp.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de gestão de contratos
    /// </summary>
    public interface IContractManagementService
    {
        /// <summary>
        /// Adiciona um novo contrato
        /// </summary>
        /// <param name="createContractDto">Dados do contrato a ser criado</param>
        /// <returns>Dados do contrato criado</returns>
        Task<ContractDto> AddContractAsync(CreateContractDto createContractDto);
        
        /// <summary>
        /// Obtém todos os contratos
        /// </summary>
        /// <returns>Lista de contratos</returns>
        Task<IEnumerable<ContractDto>> GetAllContractsAsync();
        
        /// <summary>
        /// Obtém um contrato pelo ID
        /// </summary>
        /// <param name="id">ID do contrato</param>
        /// <returns>Dados do contrato</returns>
        Task<ContractDto> GetContractByIdAsync(int id);
        
        /// <summary>
        /// Atualiza um contrato existente
        /// </summary>
        /// <param name="contractDto">Dados do contrato a ser atualizado</param>
        /// <returns>Dados do contrato atualizado</returns>
        Task<ContractDto> UpdateContractAsync(ContractDto contractDto);
        
        /// <summary>
        /// Remove um contrato
        /// </summary>
        /// <param name="id">ID do contrato</param>
        /// <returns>True se removido com sucesso</returns>
        Task<bool> RemoveContractAsync(int id);
        
        /// <summary>
        /// Renova um contrato existente
        /// </summary>
        /// <param name="id">ID do contrato</param>
        /// <param name="newEndDate">Nova data de término</param>
        /// <returns>Dados do contrato renovado</returns>
        Task<ContractDto> RenewContractAsync(int id, DateTime newEndDate);
        
        /// <summary>
        /// Obtém contratos próximos da expiração
        /// </summary>
        /// <param name="daysThreshold">Limite de dias para expiração</param>
        /// <returns>Lista de contratos próximos da expiração</returns>
        Task<IEnumerable<ContractDto>> GetContractsNearExpirationAsync(int daysThreshold);
    }
}