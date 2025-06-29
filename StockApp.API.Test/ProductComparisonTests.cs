using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Linq;

namespace StockApp.API.Test
{
    public class ProductComparisonTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductsController _controller;

        public ProductComparisonTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockCacheService = new Mock<ICacheService>();
            _mockProductRepository = new Mock<IProductRepository>();
            _controller = new ProductsController(_mockProductService.Object, _mockProductRepository.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task CompareProducts_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var request = new ProductComparisonRequestDTO
            {
                ProductIds = new List<int> { 1, 2, 3 }
            };

            var expectedComparison = new ProductComparisonDTO
            {
                Products = new List<ProductDTO>
                {
                    new ProductDTO { Id = 1, Name = "Produto 1", Price = 10.00m, Stock = 100 },
                    new ProductDTO { Id = 2, Name = "Produto 2", Price = 20.00m, Stock = 50 },
                    new ProductDTO { Id = 3, Name = "Produto 3", Price = 15.00m, Stock = 75 }
                },
                Summary = new ProductComparisonSummaryDTO
                {
                    TotalProductsCompared = 3,
                    HighestPrice = 20.00m,
                    LowestPrice = 10.00m,
                    AveragePrice = 15.00m,
                    PriceDifference = 10.00m,
                    HighestStock = 100,
                    LowestStock = 50,
                    AverageStock = 75.0
                },
                ComparedAt = DateTime.UtcNow
            };

            _mockProductService.Setup(x => x.CompareProductsAsync(request.ProductIds))
                              .ReturnsAsync(expectedComparison);

            // Act
            var result = await _controller.CompareProducts(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comparison = Assert.IsType<ProductComparisonDTO>(okResult.Value);
            Assert.Equal(3, comparison.Products.Count);
            Assert.Equal(3, comparison.Summary.TotalProductsCompared);
        }

        [Fact]
        public async Task CompareProducts_NullRequest_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.CompareProducts(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("É necessário pelo menos 2 produtos para comparação.", badRequestResult.Value);
        }

        [Fact]
        public async Task CompareProducts_EmptyProductIds_ReturnsBadRequest()
        {
            // Arrange
            var request = new ProductComparisonRequestDTO
            {
                ProductIds = new List<int>()
            };

            // Act
            var result = await _controller.CompareProducts(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("É necessário pelo menos 2 produtos para comparação.", badRequestResult.Value);
        }

        [Fact]
        public async Task CompareProducts_SingleProduct_ReturnsBadRequest()
        {
            // Arrange
            var request = new ProductComparisonRequestDTO
            {
                ProductIds = new List<int> { 1 }
            };

            // Act
            var result = await _controller.CompareProducts(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("É necessário pelo menos 2 produtos para comparação.", badRequestResult.Value);
        }

        [Fact]
        public async Task CompareProducts_TooManyProducts_ReturnsBadRequest()
        {
            // Arrange
            var request = new ProductComparisonRequestDTO
            {
                ProductIds = Enumerable.Range(1, 11).ToList() // 11 produtos
            };

            // Act
            var result = await _controller.CompareProducts(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Máximo de 10 produtos podem ser comparados por vez.", badRequestResult.Value);
        }

        [Fact]
        public async Task CompareProducts_ServiceThrowsArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var request = new ProductComparisonRequestDTO
            {
                ProductIds = new List<int> { 1, 2 }
            };

            _mockProductService.Setup(x => x.CompareProductsAsync(request.ProductIds))
                              .ThrowsAsync(new ArgumentException("Produtos não encontrados"));

            // Act
            var result = await _controller.CompareProducts(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Produtos não encontrados", badRequestResult.Value);
        }

        [Fact]
        public async Task CompareProducts_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var request = new ProductComparisonRequestDTO
            {
                ProductIds = new List<int> { 1, 2 }
            };

            _mockProductService.Setup(x => x.CompareProductsAsync(request.ProductIds))
                              .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.CompareProducts(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno do servidor ao comparar produtos.", statusCodeResult.Value);
        }

        [Fact]
        public async Task CompareProductsByCategory_ValidCategory_ReturnsOkResult()
        {
            // Arrange
            int categoryId = 1;
            int limit = 5;

            var searchResult = new PagedResult<ProductDTO>(
                new List<ProductDTO>
                {
                    new ProductDTO { Id = 1, Name = "Produto 1", Price = 10.00m, Stock = 100 },
                    new ProductDTO { Id = 2, Name = "Produto 2", Price = 20.00m, Stock = 50 }
                },
                1, limit, 2
            );

            var expectedComparison = new ProductComparisonDTO
            {
                Products = searchResult.Data.ToList(),
                Summary = new ProductComparisonSummaryDTO
                {
                    TotalProductsCompared = 2,
                    HighestPrice = 20.00m,
                    LowestPrice = 10.00m,
                    AveragePrice = 15.00m
                }
            };

            _mockProductService.Setup(x => x.GetProductsWithFiltersAsync(It.IsAny<ProductSearchDTO>()))
                              .ReturnsAsync(searchResult);

            _mockProductService.Setup(x => x.CompareProductsAsync(It.IsAny<List<int>>()))
                              .ReturnsAsync(expectedComparison);

            // Act
            var result = await _controller.CompareProductsByCategory(categoryId, limit);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comparison = Assert.IsType<ProductComparisonDTO>(okResult.Value);
            Assert.Equal(2, comparison.Products.Count);
        }

        [Fact]
        public async Task CompareProductsByCategory_InsufficientProducts_ReturnsBadRequest()
        {
            // Arrange
            int categoryId = 1;
            int limit = 5;

            var searchResult = new PagedResult<ProductDTO>(
                new List<ProductDTO>
                {
                    new ProductDTO { Id = 1, Name = "Produto 1", Price = 10.00m, Stock = 100 }
                },
                1, limit, 1
            );

            _mockProductService.Setup(x => x.GetProductsWithFiltersAsync(It.IsAny<ProductSearchDTO>()))
                              .ReturnsAsync(searchResult);

            // Act
            var result = await _controller.CompareProductsByCategory(categoryId, limit);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Categoria não possui produtos suficientes para comparação.", badRequestResult.Value);
        }

        [Theory]
        [InlineData(1, 2)] // Limite mínimo
        [InlineData(15, 10)] // Limite máximo
        [InlineData(0, 2)] // Limite inválido
        public async Task CompareProductsByCategory_LimitValidation_AdjustsLimitCorrectly(int inputLimit, int expectedLimit)
        {
            // Arrange
            int categoryId = 1;

            var searchResult = new PagedResult<ProductDTO>(
                new List<ProductDTO>
                {
                    new ProductDTO { Id = 1, Name = "Produto 1", Price = 10.00m, Stock = 100 },
                    new ProductDTO { Id = 2, Name = "Produto 2", Price = 20.00m, Stock = 50 }
                },
                1, expectedLimit, 2
            );

            var expectedComparison = new ProductComparisonDTO
            {
                Products = searchResult.Data.ToList(),
                Summary = new ProductComparisonSummaryDTO { TotalProductsCompared = 2 }
            };

            _mockProductService.Setup(x => x.GetProductsWithFiltersAsync(It.Is<ProductSearchDTO>(p => p.PageSize == expectedLimit)))
                              .ReturnsAsync(searchResult);

            _mockProductService.Setup(x => x.CompareProductsAsync(It.IsAny<List<int>>()))
                              .ReturnsAsync(expectedComparison);

            // Act
            var result = await _controller.CompareProductsByCategory(categoryId, inputLimit);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            _mockProductService.Verify(x => x.GetProductsWithFiltersAsync(It.Is<ProductSearchDTO>(p => p.PageSize == expectedLimit)), Times.Once);
        }
    }
}