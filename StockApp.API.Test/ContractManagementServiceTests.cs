using AutoMapper;
using Moq;
using StockApp.Application.DTOs;
using StockApp.Application.Mappings;
using StockApp.Application.Services;
using StockApp.Domain.Entities;
using StockApp.Domain.Enums;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StockApp.API.Test
{
    public class ContractManagementServiceTests
    {
        private readonly Mock<IContractRepository> _contractRepositoryMock;
        private readonly Mock<ISupplierRepository> _supplierRepositoryMock;
        private readonly IMapper _mapper;
        private readonly ContractManagementService _contractManagementService;

        public ContractManagementServiceTests()
        {
            _contractRepositoryMock = new Mock<IContractRepository>();
            _supplierRepositoryMock = new Mock<ISupplierRepository>();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToDTOMappingProfile>();
            });
            _mapper = config.CreateMapper();
            
            _contractManagementService = new ContractManagementService(
                _contractRepositoryMock.Object,
                _supplierRepositoryMock.Object,
                _mapper);
        }

        [Fact]
        public async Task AddContractAsync_ValidSupplierContract_ShouldCreateContract()
        {
            // Arrange
            var supplierId = 1;
            var createContractDto = new CreateContractDto
            {
                SupplierId = supplierId,
                ContractNumber = "CONT-2023-001",
                Description = "Contrato de fornecimento",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddYears(1),
                Value = 10000m,
                Terms = "Termos e condições do contrato",
                ContractType = ContractType.Supplier
            };

            var supplier = new Supplier(supplierId, "Fornecedor Teste", "12345678901234", "contato@fornecedor.com", "11999999999");
            var contract = new Contract(
                supplierId,
                null,
                createContractDto.ContractNumber,
                createContractDto.Description,
                createContractDto.StartDate,
                createContractDto.EndDate,
                createContractDto.Value,
                createContractDto.Terms,
                ContractType.Supplier);

            _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId))
                .ReturnsAsync(supplier);

            _contractRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Contract>()))
                .ReturnsAsync(contract);

            // Act
            var result = await _contractManagementService.AddContractAsync(createContractDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(supplierId, result.SupplierId);
            Assert.Equal(createContractDto.ContractNumber, result.ContractNumber);
            Assert.Equal(createContractDto.Description, result.Description);
            Assert.Equal(createContractDto.StartDate, result.StartDate);
            Assert.Equal(createContractDto.EndDate, result.EndDate);
            Assert.Equal(createContractDto.Value, result.Value);
            Assert.Equal(createContractDto.Terms, result.Terms);
            Assert.Equal(ContractType.Supplier, result.ContractType);
            Assert.True(result.IsActive);

            _contractRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Contract>()), Times.Once);
        }

        [Fact]
        public async Task AddContractAsync_InvalidSupplier_ShouldThrowException()
        {
            // Arrange
            var supplierId = 999; // ID de fornecedor inexistente
            var createContractDto = new CreateContractDto
            {
                SupplierId = supplierId,
                ContractNumber = "CONT-2023-001",
                Description = "Contrato de fornecimento",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddYears(1),
                Value = 10000m,
                Terms = "Termos e condições do contrato",
                ContractType = ContractType.Supplier
            };

            _supplierRepositoryMock.Setup(x => x.GetByIdAsync(supplierId))
                .ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => 
                _contractManagementService.AddContractAsync(createContractDto));

            _contractRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Contract>()), Times.Never);
        }

        [Fact]
        public async Task GetContractByIdAsync_ExistingContract_ShouldReturnContract()
        {
            // Arrange
            var contractId = 1;
            var contract = new Contract(
                1,
                null,
                "CONT-2023-001",
                "Contrato de fornecimento",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddYears(1),
                10000m,
                "Termos e condições do contrato",
                ContractType.Supplier);

            _contractRepositoryMock.Setup(x => x.GetByIdAsync(contractId))
                .ReturnsAsync(contract);

            // Act
            var result = await _contractManagementService.GetContractByIdAsync(contractId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contract.SupplierId, result.SupplierId);
            Assert.Equal(contract.ContractNumber, result.ContractNumber);
            Assert.Equal(contract.Description, result.Description);
            Assert.Equal(contract.StartDate, result.StartDate);
            Assert.Equal(contract.EndDate, result.EndDate);
            Assert.Equal(contract.Value, result.Value);
            Assert.Equal(contract.Terms, result.Terms);
            Assert.Equal(contract.ContractType, result.ContractType);
        }

        [Fact]
        public async Task GetContractByIdAsync_NonExistingContract_ShouldThrowException()
        {
            // Arrange
            var contractId = 999; // ID de contrato inexistente

            _contractRepositoryMock.Setup(x => x.GetByIdAsync(contractId))
                .ReturnsAsync((Contract)null);

            // Act & Assert
            await Assert.ThrowsAsync<ApplicationException>(() => 
                _contractManagementService.GetContractByIdAsync(contractId));
        }

        [Fact]
        public async Task RenewContractAsync_ValidContract_ShouldRenewContract()
        {
            // Arrange
            var contractId = 1;
            var newEndDate = DateTime.Now.AddYears(2);
            var contract = new Contract(
                1,
                null,
                "CONT-2023-001",
                "Contrato de fornecimento",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddYears(1),
                10000m,
                "Termos e condições do contrato",
                ContractType.Supplier);

            _contractRepositoryMock.Setup(x => x.GetByIdAsync(contractId))
                .ReturnsAsync(contract);

            _contractRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Contract>()))
                .ReturnsAsync((Contract c) => c);

            // Act
            var result = await _contractManagementService.RenewContractAsync(contractId, newEndDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newEndDate, result.EndDate);
            Assert.True(result.IsActive);

            _contractRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Contract>()), Times.Once);
        }

        [Fact]
        public async Task GetContractsNearExpirationAsync_ShouldReturnNearExpirationContracts()
        {
            // Arrange
            var daysThreshold = 30;
            var contracts = new List<Contract>
            {
                new Contract(1, null, "CONT-2023-001", "Contrato 1", DateTime.Now.AddDays(-30), DateTime.Now.AddDays(15), 10000m, "Termos 1", ContractType.Supplier),
                new Contract(2, null, "CONT-2023-002", "Contrato 2", DateTime.Now.AddDays(-60), DateTime.Now.AddDays(25), 20000m, "Termos 2", ContractType.Supplier),
                new Contract(3, null, "CONT-2023-003", "Contrato 3", DateTime.Now.AddDays(-90), DateTime.Now.AddDays(45), 30000m, "Termos 3", ContractType.Supplier)
            };

            _contractRepositoryMock.Setup(x => x.GetContractsNearExpirationAsync(daysThreshold))
                .ReturnsAsync(contracts.Where(c => (c.EndDate - DateTime.Now).TotalDays <= daysThreshold).ToList());

            // Act
            var result = await _contractManagementService.GetContractsNearExpirationAsync(daysThreshold);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Apenas os contratos 1 e 2 estão próximos da expiração
            Assert.Contains(result, c => c.ContractNumber == "CONT-2023-001");
            Assert.Contains(result, c => c.ContractNumber == "CONT-2023-002");
            Assert.DoesNotContain(result, c => c.ContractNumber == "CONT-2023-003");
        }
    }
}