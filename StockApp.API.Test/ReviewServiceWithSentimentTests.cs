using AutoMapper;
using Moq;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Application.Mappings;
using StockApp.Application.Services;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace StockApp.API.Test
{
    public class ReviewServiceWithSentimentTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ISentimentAnalysisService> _sentimentAnalysisServiceMock;
        private readonly IMapper _mapper;
        private readonly ReviewService _reviewService;

        public ReviewServiceWithSentimentTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _sentimentAnalysisServiceMock = new Mock<ISentimentAnalysisService>();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToDTOMappingProfile>();
            });
            _mapper = config.CreateMapper();
            
            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object, 
                _productRepositoryMock.Object, 
                _mapper,
                _sentimentAnalysisServiceMock.Object);
        }

        [Fact]
        public async Task GetReviewByIdAsync_ShouldIncludeSentimentAnalysis()
        {
            // Arrange
            var reviewId = 1;
            var review = new Review(1, 1, "user123", 5, "Excelente produto!", DateTime.UtcNow, true);

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId))
                .ReturnsAsync(review);

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync(review.Comment))
                .ReturnsAsync("Positivo");

            // Act
            var result = await _reviewService.GetReviewByIdAsync(reviewId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Positivo", result.SentimentAnalysis);
        }

        [Fact]
        public async Task CreateReviewAsync_ShouldIncludeSentimentAnalysis()
        {
            // Arrange
            var productId = 1;
            var userId = "user123";
            var createReviewDto = new CreateReviewDTO
            {
                Rating = 5,
                Comment = "Excelente produto!"
            };

            var product = new Product(1, "Produto Teste", "Descrição", 100m, 10, "image.jpg", 1);
            var review = new Review(productId, userId, createReviewDto.Rating, createReviewDto.Comment);

            _productRepositoryMock.Setup(x => x.GetById(productId))
                .ReturnsAsync(product);
            
            _reviewRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Review>());
            
            _reviewRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Review>()))
                .ReturnsAsync(review);

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync(createReviewDto.Comment))
                .ReturnsAsync("Positivo");

            // Act
            var result = await _reviewService.CreateReviewAsync(productId, userId, createReviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Positivo", result.SentimentAnalysis);
        }

        [Fact]
        public async Task GetAllReviewsAsync_ShouldIncludeSentimentAnalysis()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review(1, 1, "user1", 5, "Excelente produto!", DateTime.UtcNow, true),
                new Review(2, 2, "user2", 2, "Produto ruim, não recomendo.", DateTime.UtcNow, true)
            };

            _reviewRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(reviews);

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync("Excelente produto!"))
                .ReturnsAsync("Positivo");

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync("Produto ruim, não recomendo."))
                .ReturnsAsync("Negativo");

            // Act
            var result = await _reviewService.GetAllReviewsAsync();

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Positivo", resultList[0].SentimentAnalysis);
            Assert.Equal("Negativo", resultList[1].SentimentAnalysis);
        }

        [Fact]
        public async Task GetReviewsByProductIdAsync_ShouldIncludeSentimentAnalysis()
        {
            // Arrange
            var productId = 1;
            var reviews = new List<Review>
            {
                new Review(1, productId, "user1", 5, "Excelente produto!", DateTime.UtcNow, true),
                new Review(2, productId, "user2", 3, "Produto mediano.", DateTime.UtcNow, true)
            };

            _reviewRepositoryMock.Setup(x => x.GetApprovedReviewsByProductIdAsync(productId))
                .ReturnsAsync(reviews);

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync("Excelente produto!"))
                .ReturnsAsync("Positivo");

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync("Produto mediano."))
                .ReturnsAsync("Neutro");

            // Act
            var result = await _reviewService.GetReviewsByProductIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Positivo", resultList[0].SentimentAnalysis);
            Assert.Equal("Neutro", resultList[1].SentimentAnalysis);
        }

        [Fact]
        public async Task UpdateReviewAsync_ShouldUpdateSentimentAnalysis()
        {
            // Arrange
            var reviewId = 1;
            var userId = "user123";
            var updateReviewDto = new UpdateReviewDTO
            {
                Rating = 4,
                Comment = "Produto atualizado - muito bom!"
            };

            var existingReview = new Review(1, 1, userId, 3, "Comentário antigo", DateTime.UtcNow, false);

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId))
                .ReturnsAsync(existingReview);
            
            _reviewRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Review>()))
                .ReturnsAsync(existingReview);

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync(updateReviewDto.Comment))
                .ReturnsAsync("Positivo");

            // Act
            var result = await _reviewService.UpdateReviewAsync(reviewId, userId, updateReviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Positivo", result.SentimentAnalysis);
        }
    }
}