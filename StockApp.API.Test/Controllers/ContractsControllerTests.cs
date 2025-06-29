using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StockApp.API.Test.Controllers
{
    public class ContractsControllerTests
    {
        private readonly Mock<IContractManagementService> _contractManagementServiceMock;
        private readonly ContractsController _controller;

        public ContractsControllerTests()
        {
            _contractManagementServiceMock = new Mock<IContractManagementService>();
            _controller = new ContractsController(_contractManagementServiceMock.Object);
        }

        [Fact]
        public async Task GetAllContracts_ReturnsOkResult_WithContracts()
        {
            // Arrange
            var contracts = new List<ContractDto>
            {
                new ContractDto
                {
                    Id = 1,
                    SupplierId = 1,
                    ContractNumber = "CONT-2023-001",
                    Description = "Contrato de fornecimento",
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddYears(1),
                    Value = 10000m,
                    Terms = "Termos e condições",
                    IsActive = true,
                    ContractType = ContractType.Supplier
                },
                new ContractDto
                {
                    Id = 2,
                    ClientId = 1,
                    ContractNumber = "CONT-2023-002",
                    Description = "Contrato de serviço",
                    StartDate = DateTime.Now.AddDays(-60),
                    EndDate = DateTime.Now.AddMonths(6),
                    Value = 5000m,
                    Terms = "Termos e condições",
                    IsActive = true,
                    ContractType = ContractType.Client
                }
            };

            _contractManagementServiceMock.Setup(s => s.GetAllContractsAsync())
                .ReturnsAsync(contracts);

            // Act
            var result = await _controller.GetAllContracts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContracts = Assert.IsAssignableFrom<IEnumerable<ContractDto>>(okResult.Value);
            Assert.Equal(2, returnedContracts.Count());
        }

        [Fact]
        public async Task GetContractById_ReturnsOkResult_WithContract()
        {
            // Arrange
            var contractId = 1;
            var contract = new ContractDto
            {
                Id = contractId,
                SupplierId = 1,
                ContractNumber = "CONT-2023-001",
                Description = "Contrato de fornecimento",
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddYears(1),
                Value = 10000m,
                Terms = "Termos e condições",
                IsActive = true,
                ContractType = ContractType.Supplier
            };

            _contractManagementServiceMock.Setup(s => s.GetContractByIdAsync(contractId))
                .ReturnsAsync(contract);

            // Act
            var result = await _controller.GetContractById(contractId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContract = Assert.IsType<ContractDto>(okResult.Value);
            Assert.Equal(contractId, returnedContract.Id);
        }

        [Fact]
        public async Task GetContractById_ReturnsNotFound_WhenContractDoesNotExist()
        {
            // Arrange
            var contractId = 999;
            _contractManagementServiceMock.Setup(s => s.GetContractByIdAsync(contractId))
                .ThrowsAsync(new ApplicationException($"Contrato com ID {contractId} não encontrado."));

            // Act
            var result = await _controller.GetContractById(contractId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddContract_ReturnsCreatedAtAction_WithNewContract()
        {
            // Arrange
            var createContractDto = new CreateContractDto
            {
                SupplierId = 1,
                ContractNumber = "CONT-2023-001",
                Description = "Contrato de fornecimento",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddYears(1),
                Value = 10000m,
                Terms = "Termos e condições",
                ContractType = ContractType.Supplier
            };

            var createdContract = new ContractDto
            {
                Id = 1,
                SupplierId = createContractDto.SupplierId,
                ContractNumber = createContractDto.ContractNumber,
                Description = createContractDto.Description,
                StartDate = createContractDto.StartDate,
                EndDate = createContractDto.EndDate,
                Value = createContractDto.Value,
                Terms = createContractDto.Terms,
                IsActive = true,
                ContractType = createContractDto.ContractType,
                CreatedAt = DateTime.Now
            };

            _contractManagementServiceMock.Setup(s => s.AddContractAsync(createContractDto))
                .ReturnsAsync(createdContract);

            // Act
            var result = await _controller.AddContract(createContractDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedContract = Assert.IsType<ContractDto>(createdAtActionResult.Value);
            Assert.Equal(createdContract.Id, returnedContract.Id);
            Assert.Equal(createContractDto.ContractNumber, returnedContract.ContractNumber);
        }

        [Fact]
        public async Task UpdateContract_ReturnsOkResult_WithUpdatedContract()
        {
            // Arrange
            var contractId = 1;
            var contractDto = new ContractDto
            {
                Id = contractId,
                SupplierId = 1,
                ContractNumber = "CONT-2023-001",
                Description = "Contrato de fornecimento atualizado",
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now.AddYears(2),
                Value = 15000m,
                Terms = "Termos e condições atualizados",
                IsActive = true,
                ContractType = ContractType.Supplier
            };

            _contractManagementServiceMock.Setup(s => s.UpdateContractAsync(contractDto))
                .ReturnsAsync(contractDto);

            // Act
            var result = await _controller.UpdateContract(contractId, contractDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContract = Assert.IsType<ContractDto>(okResult.Value);
            Assert.Equal(contractId, returnedContract.Id);
            Assert.Equal(contractDto.Description, returnedContract.Description);
            Assert.Equal(contractDto.Value, returnedContract.Value);
        }

        [Fact]
        public async Task UpdateContract_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var contractId = 1;
            var contractDto = new ContractDto { Id = 2 }; // ID diferente

            // Act
            var result = await _controller.UpdateContract(contractId, contractDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task RemoveContract_ReturnsOkResult_WhenContractIsRemoved()
        {
            // Arrange
            var contractId = 1;
            _contractManagementServiceMock.Setup(s => s.RemoveContractAsync(contractId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveContract(contractId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains($"Contrato com ID {contractId} removido com sucesso", okResult.Value.ToString());
        }

        [Fact]
        public async Task RemoveContract_ReturnsNotFound_WhenContractDoesNotExist()
        {
            // Arrange
            var contractId = 999;
            _contractManagementServiceMock.Setup(s => s.RemoveContractAsync(contractId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveContract(contractId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task RenewContract_ReturnsOkResult_WithRenewedContract()
        {
            // Arrange
            var contractId = 1;
            var newEndDate = DateTime.Now.AddYears(2);
            var renewedContract = new ContractDto
            {
                Id = contractId,
                SupplierId = 1,
                ContractNumber = "CONT-2023-001",
                Description = "Contrato de fornecimento",
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = newEndDate,
                Value = 10000m,
                Terms = "Termos e condições",
                IsActive = true,
                ContractType = ContractType.Supplier
            };

            _contractManagementServiceMock.Setup(s => s.RenewContractAsync(contractId, newEndDate))
                .ReturnsAsync(renewedContract);

            // Act
            var result = await _controller.RenewContract(contractId, newEndDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContract = Assert.IsType<ContractDto>(okResult.Value);
            Assert.Equal(contractId, returnedContract.Id);
            Assert.Equal(newEndDate, returnedContract.EndDate);
        }

        [Fact]
        public async Task GetContractsNearExpiration_ReturnsOkResult_WithNearExpirationContracts()
        {
            // Arrange
            var daysThreshold = 30;
            var contracts = new List<ContractDto>
            {
                new ContractDto
                {
                    Id = 1,
                    SupplierId = 1,
                    ContractNumber = "CONT-2023-001",
                    Description = "Contrato de fornecimento",
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(15),
                    Value = 10000m,
                    Terms = "Termos e condições",
                    IsActive = true,
                    ContractType = ContractType.Supplier,
                    IsNearExpiration = true
                },
                new ContractDto
                {
                    Id = 2,
                    ClientId = 1,
                    ContractNumber = "CONT-2023-002",
                    Description = "Contrato de serviço",
                    StartDate = DateTime.Now.AddDays(-60),
                    EndDate = DateTime.Now.AddDays(25),
                    Value = 5000m,
                    Terms = "Termos e condições",
                    IsActive = true,
                    ContractType = ContractType.Client,
                    IsNearExpiration = true
                }
            };

            _contractManagementServiceMock.Setup(s => s.GetContractsNearExpirationAsync(daysThreshold))
                .ReturnsAsync(contracts);

            // Act
            var result = await _controller.GetContractsNearExpiration(daysThreshold);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedContracts = Assert.IsAssignableFrom<IEnumerable<ContractDto>>(okResult.Value);
            Assert.Equal(2, returnedContracts.Count());
            Assert.All(returnedContracts, c => Assert.True(c.IsNearExpiration));
        }
    }
}