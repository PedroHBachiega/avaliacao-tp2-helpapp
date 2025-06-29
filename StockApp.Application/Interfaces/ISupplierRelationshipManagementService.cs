using StockApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de gestão de relacionamento com fornecedores
    /// </summary>
    public interface ISupplierRelationshipManagementService
    {
        /// <summary>
        /// Avalia um fornecedor
        /// </summary>
        /// <param name="supplierId">ID do fornecedor</param>
        /// <returns>Dados do fornecedor avaliado</returns>
        Task<SupplierDTO> EvaluateSupplierAsync(int supplierId);
        
        /// <summary>
        /// Adiciona uma nova avaliação para um fornecedor
        /// </summary>
        /// <param name="evaluationDto">Dados da avaliação</param>
        /// <returns>Dados da avaliação criada</returns>
        Task<SupplierEvaluationDto> AddEvaluationAsync(SupplierEvaluationDto evaluationDto);
        
        /// <summary>
        /// Obtém todas as avaliações de um fornecedor
        /// </summary>
        /// <param name="supplierId">ID do fornecedor</param>
        /// <returns>Lista de avaliações do fornecedor</returns>
        Task<IEnumerable<SupplierEvaluationDto>> GetSupplierEvaluationsAsync(int supplierId);
        
        /// <summary>
        /// Renova um contrato de fornecedor
        /// </summary>
        /// <param name="contractId">ID do contrato</param>
        /// <param name="newEndDate">Nova data de término</param>
        /// <returns>Dados do contrato renovado</returns>
        Task<SupplierContractDto> RenewContractAsync(int contractId, DateTime newEndDate);
        
        /// <summary>
        /// Verifica contratos próximos do vencimento
        /// </summary>
        /// <param name="daysThreshold">Limite de dias para considerar próximo do vencimento</param>
        /// <returns>Lista de contratos próximos do vencimento</returns>
        Task<IEnumerable<SupplierContractDto>> GetContractsNearExpirationAsync(int daysThreshold);
    }
}