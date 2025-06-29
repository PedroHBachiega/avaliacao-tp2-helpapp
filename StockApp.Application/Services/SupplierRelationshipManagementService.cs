using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    /// <summary>
    /// Serviço para gestão de relacionamento com fornecedores
    /// </summary>
    public class SupplierRelationshipManagementService : ISupplierRelationshipManagementService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierContractRepository _contractRepository;
        private readonly ISupplierEvaluationRepository _evaluationRepository;
        private readonly IMapper _mapper;

        public SupplierRelationshipManagementService(
            ISupplierRepository supplierRepository,
            ISupplierContractRepository contractRepository,
            ISupplierEvaluationRepository evaluationRepository,
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _contractRepository = contractRepository;
            _evaluationRepository = evaluationRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Avalia um fornecedor calculando a média de suas avaliações
        /// </summary>
        public async Task<SupplierDTO> EvaluateSupplierAsync(int supplierId)
        {
            // Verifica se o fornecedor existe
            var supplier = await _supplierRepository.GetById(supplierId);
            if (supplier == null)
            {
                throw new ApplicationException($"Fornecedor com ID {supplierId} não encontrado.");
            }

            // Obtém todas as avaliações do fornecedor
            var evaluations = await _evaluationRepository.GetBySupplierId(supplierId);
            
            // Calcula a pontuação média
            int evaluationScore = 0;
            if (evaluations.Any())
            {
                evaluationScore = (int)evaluations.Average(e => e.Score);
            }

            // Mapeia o fornecedor para DTO e adiciona a pontuação
            var supplierDto = _mapper.Map<SupplierDTO>(supplier);
            supplierDto.EvaluationScore = evaluationScore;

            return supplierDto;
        }

        /// <summary>
        /// Adiciona uma nova avaliação para um fornecedor
        /// </summary>
        public async Task<SupplierEvaluationDto> AddEvaluationAsync(SupplierEvaluationDto evaluationDto)
        {
            // Verifica se o fornecedor existe
            var supplier = await _supplierRepository.GetById(evaluationDto.SupplierId);
            if (supplier == null)
            {
                throw new ApplicationException($"Fornecedor com ID {evaluationDto.SupplierId} não encontrado.");
            }

            // Mapeia o DTO para a entidade
            var evaluation = new SupplierEvaluation(
                evaluationDto.Category,
                evaluationDto.Comment,
                evaluationDto.Score,
                evaluationDto.EvaluatedBy);

            // Define o ID do fornecedor
            var evaluationWithSupplierId = new SupplierEvaluation(
                0,
                evaluationDto.SupplierId,
                evaluationDto.Category,
                evaluationDto.Comment,
                evaluationDto.Score,
                evaluationDto.EvaluatedBy,
                DateTime.Now);

            // Adiciona a avaliação
            var createdEvaluation = await _evaluationRepository.CreateAsync(evaluationWithSupplierId);

            // Mapeia a entidade criada de volta para DTO
            return _mapper.Map<SupplierEvaluationDto>(createdEvaluation);
        }

        /// <summary>
        /// Obtém todas as avaliações de um fornecedor
        /// </summary>
        public async Task<IEnumerable<SupplierEvaluationDto>> GetSupplierEvaluationsAsync(int supplierId)
        {
            // Verifica se o fornecedor existe
            var supplier = await _supplierRepository.GetById(supplierId);
            if (supplier == null)
            {
                throw new ApplicationException($"Fornecedor com ID {supplierId} não encontrado.");
            }

            // Obtém todas as avaliações do fornecedor
            var evaluations = await _evaluationRepository.GetBySupplierId(supplierId);

            // Mapeia as entidades para DTOs
            return _mapper.Map<IEnumerable<SupplierEvaluationDto>>(evaluations);
        }

        /// <summary>
        /// Renova um contrato de fornecedor
        /// </summary>
        public async Task<SupplierContractDto> RenewContractAsync(int contractId, DateTime newEndDate)
        {
            // Verifica se o contrato existe
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
            {
                throw new ApplicationException($"Contrato com ID {contractId} não encontrado.");
            }

            // Verifica se a nova data de término é válida
            if (newEndDate <= DateTime.Now)
            {
                throw new ApplicationException("A nova data de término deve ser maior que a data atual.");
            }

            // Atualiza o contrato
            contract.Update(contract.Description, newEndDate, contract.Value, true);
            var updatedContract = await _contractRepository.UpdateAsync(contract);

            // Mapeia a entidade atualizada para DTO
            return _mapper.Map<SupplierContractDto>(updatedContract);
        }

        /// <summary>
        /// Verifica contratos próximos do vencimento
        /// </summary>
        public async Task<IEnumerable<SupplierContractDto>> GetContractsNearExpirationAsync(int daysThreshold)
        {
            // Calcula a data limite
            var limitDate = DateTime.Now.AddDays(daysThreshold);

            // Obtém todos os contratos ativos
            var allContracts = await _contractRepository.GetAllAsync();

            // Filtra os contratos que estão próximos do vencimento
            var contractsNearExpiration = allContracts
                .Where(c => c.IsActive && c.EndDate <= limitDate && c.EndDate > DateTime.Now)
                .ToList();

            // Mapeia as entidades para DTOs
            return _mapper.Map<IEnumerable<SupplierContractDto>>(contractsNearExpiration);
        }
    }
}