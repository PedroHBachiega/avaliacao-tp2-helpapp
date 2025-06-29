using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    /// <summary>
    /// Serviço para gestão de contratos
    /// </summary>
    public class ContractManagementService : IContractManagementService
    {
        private readonly IContractRepository _contractRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public ContractManagementService(
            IContractRepository contractRepository,
            ISupplierRepository supplierRepository,
            IMapper mapper)
        {
            _contractRepository = contractRepository;
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<ContractDto> AddContractAsync(CreateContractDto createContractDto)
        {
            // Validar se o fornecedor ou cliente existe
            if (createContractDto.SupplierId.HasValue)
            {
                var supplier = await _supplierRepository.GetById(createContractDto.SupplierId.Value);
                if (supplier == null)
                    throw new ApplicationException($"Fornecedor com ID {createContractDto.SupplierId} não encontrado.");
            }
            
            // Criar novo contrato
            var contract = new Contract(
                createContractDto.ContractNumber,
                createContractDto.Description,
                createContractDto.StartDate,
                createContractDto.EndDate,
                createContractDto.Value,
                createContractDto.Terms,
                (int)createContractDto.ContractType);

            // Definir o ID do fornecedor ou cliente
            var newContract = new Contract(
                0,
                createContractDto.SupplierId,
                createContractDto.ClientId,
                createContractDto.ContractNumber,
                createContractDto.Description,
                createContractDto.StartDate,
                createContractDto.EndDate,
                createContractDto.Value,
                createContractDto.Terms,
                true,
                (int)createContractDto.ContractType);

            var createdContract = await _contractRepository.CreateAsync(newContract);
            
            // Mapear para DTO de resposta
            var contractDto = new ContractDto
            {
                Id = createdContract.Id,
                SupplierId = createdContract.SupplierId,
                ClientId = createdContract.ClientId,
                ContractNumber = createdContract.ContractNumber,
                Description = createdContract.Description,
                StartDate = createdContract.StartDate,
                EndDate = createdContract.EndDate,
                Value = createdContract.Value,
                Terms = createdContract.Terms,
                IsActive = createdContract.IsActive,
                ContractType = (ContractType)createdContract.ContractType,
                CreatedAt = createdContract.CreatedAt
            };

            return contractDto;
        }

        public async Task<IEnumerable<ContractDto>> GetAllContractsAsync()
        {
            var contracts = await _contractRepository.GetAllAsync();
            
            // Mapear para DTOs
            var contractDtos = new List<ContractDto>();
            foreach (var contract in contracts)
            {
                contractDtos.Add(new ContractDto
                {
                    Id = contract.Id,
                    SupplierId = contract.SupplierId,
                    ClientId = contract.ClientId,
                    ContractNumber = contract.ContractNumber,
                    Description = contract.Description,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Value = contract.Value,
                    Terms = contract.Terms,
                    IsActive = contract.IsActive,
                    ContractType = (ContractType)contract.ContractType,
                    CreatedAt = contract.CreatedAt,
                    UpdatedAt = contract.UpdatedAt
                });
            }

            return contractDtos;
        }

        public async Task<ContractDto> GetContractByIdAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                throw new ApplicationException($"Contrato com ID {id} não encontrado.");
            
            // Mapear para DTO
            var contractDto = new ContractDto
            {
                Id = contract.Id,
                SupplierId = contract.SupplierId,
                ClientId = contract.ClientId,
                ContractNumber = contract.ContractNumber,
                Description = contract.Description,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Value = contract.Value,
                Terms = contract.Terms,
                IsActive = contract.IsActive,
                ContractType = (ContractType)contract.ContractType,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt
            };

            return contractDto;
        }

        public async Task<ContractDto> UpdateContractAsync(ContractDto contractDto)
        {
            var contract = await _contractRepository.GetByIdAsync(contractDto.Id);
            if (contract == null)
                throw new ApplicationException($"Contrato com ID {contractDto.Id} não encontrado.");

            // Atualizar contrato existente
            contract.Update(
                contractDto.Description,
                contractDto.EndDate,
                contractDto.Value,
                contractDto.Terms,
                contractDto.IsActive);

            await _contractRepository.UpdateAsync(contract);
            
            // Atualizar DTO com dados atualizados
            contractDto.UpdatedAt = DateTime.Now;
            
            return contractDto;
        }

        public async Task<bool> RemoveContractAsync(int id)
        {
            return await _contractRepository.DeleteAsync(id);
        }

        public async Task<ContractDto> RenewContractAsync(int id, DateTime newEndDate)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                throw new ApplicationException($"Contrato com ID {id} não encontrado.");

            // Renovar contrato
            contract.Renew(newEndDate);

            await _contractRepository.UpdateAsync(contract);
            
            // Mapear para DTO
            var contractDto = new ContractDto
            {
                Id = contract.Id,
                SupplierId = contract.SupplierId,
                ClientId = contract.ClientId,
                ContractNumber = contract.ContractNumber,
                Description = contract.Description,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Value = contract.Value,
                Terms = contract.Terms,
                IsActive = contract.IsActive,
                ContractType = (ContractType)contract.ContractType,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt
            };

            return contractDto;
        }

        public async Task<IEnumerable<ContractDto>> GetContractsNearExpirationAsync(int daysThreshold)
        {
            var contracts = await _contractRepository.GetContractsNearExpirationAsync(daysThreshold);
            
            // Mapear para DTOs
            var contractDtos = new List<ContractDto>();
            foreach (var contract in contracts)
            {
                contractDtos.Add(new ContractDto
                {
                    Id = contract.Id,
                    SupplierId = contract.SupplierId,
                    ClientId = contract.ClientId,
                    ContractNumber = contract.ContractNumber,
                    Description = contract.Description,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Value = contract.Value,
                    Terms = contract.Terms,
                    IsActive = contract.IsActive,
                    ContractType = (ContractType)contract.ContractType,
                    CreatedAt = contract.CreatedAt,
                    UpdatedAt = contract.UpdatedAt
                });
            }

            return contractDtos;
        }
    }
}