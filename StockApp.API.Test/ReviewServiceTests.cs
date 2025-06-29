using AutoMapper;
using Moq;
using StockApp.Application.DTOs;
using StockApp.Application.Mappings;
using StockApp.Application.Services;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StockApp.API.Test
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly IMapper _mapper;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToDTOMappingProfile>();
            });
            _mapper = config.CreateMapper();
            
            _reviewService = new ReviewService(_reviewRepositoryMock.Object, _productRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task CreateReviewAsync_ValidRequest_ShouldCreateReview()
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

            // Act
            var result = await _reviewService.CreateReviewAsync(productId, userId, createReviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(5, result.Rating);
            Assert.Equal("Excelente produto!", result.Comment);
            Assert.False(result.IsApproved);
        }

        [Fact]
        public async Task CreateReviewAsync_ProductNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var productId = 999;
            var userId = "user123";
            var createReviewDto = new CreateReviewDTO
            {
                Rating = 5,
                Comment = "Excelente produto!"
            };

            _productRepositoryMock.Setup(x => x.GetById(productId))
                .ReturnsAsync((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _reviewService.CreateReviewAsync(productId, userId, createReviewDto));
        }

        [Fact]
        public async Task CreateReviewAsync_UserAlreadyReviewed_ShouldThrowInvalidOperationException()
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
            var existingReview = new Review(productId, userId, 4, "Já avaliei antes");

            _productRepositoryMock.Setup(x => x.GetById(productId))
                .ReturnsAsync(product);
            
            _reviewRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Review> { existingReview });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _reviewService.CreateReviewAsync(productId, userId, createReviewDto));
        }

        [Fact]
        public async Task GetReviewSummaryByProductIdAsync_WithReviews_ShouldReturnCorrectSummary()
        {
            // Arrange
            var productId = 1;
            var reviews = new List<Review>
            {
                new Review(productId, "user1", 5, "Excelente"),
                new Review(productId, "user2", 4, "Muito bom"),
                new Review(productId, "user3", 5, "Perfeito"),
                new Review(productId, "user4", 3, "Regular"),
                new Review(productId, "user5", 4, "Bom")
            };

            _reviewRepositoryMock.Setup(x => x.GetApprovedReviewsByProductIdAsync(productId))
                .ReturnsAsync(reviews);

            // Act
            var result = await _reviewService.GetReviewSummaryByProductIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(4.2, result.AverageRating, 1);
            Assert.Equal(5, result.TotalReviews);
            Assert.Equal(2, result.FiveStars);
            Assert.Equal(2, result.FourStars);
            Assert.Equal(1, result.ThreeStars);
            Assert.Equal(0, result.TwoStars);
            Assert.Equal(0, result.OneStar);
        }

        [Fact]
        public async Task GetReviewSummaryByProductIdAsync_NoReviews_ShouldReturnEmptySummary()
        {
            // Arrange
            var productId = 1;
            
            _reviewRepositoryMock.Setup(x => x.GetApprovedReviewsByProductIdAsync(productId))
                .ReturnsAsync(new List<Review>());

            // Act
            var result = await _reviewService.GetReviewSummaryByProductIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(0, result.AverageRating);
            Assert.Equal(0, result.TotalReviews);
        }

        [Fact]
        public async Task UpdateReviewAsync_ValidRequest_ShouldUpdateReview()
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

            // Act
            var result = await _reviewService.UpdateReviewAsync(reviewId, userId, updateReviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Rating);
            Assert.Equal("Produto atualizado - muito bom!", result.Comment);
        }

        [Fact]
        public async Task UpdateReviewAsync_ReviewNotFound_ShouldThrowArgumentException()
        {
            // Arrange
            var reviewId = 999;
            var userId = "user123";
            var updateReviewDto = new UpdateReviewDTO
            {
                Rating = 4,
                Comment = "Comentário atualizado"
            };

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId))
                .ReturnsAsync((Review)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _reviewService.UpdateReviewAsync(reviewId, userId, updateReviewDto));
        }

        [Fact]
        public async Task UpdateReviewAsync_UnauthorizedUser_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var reviewId = 1;
            var userId = "user123";
            var wrongUserId = "wronguser";
            var updateReviewDto = new UpdateReviewDTO
            {
                Rating = 4,
                Comment = "Comentário atualizado"
            };

            var existingReview = new Review(1, 1, userId, 3, "Comentário antigo", DateTime.UtcNow, false);

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId))
                .ReturnsAsync(existingReview);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _reviewService.UpdateReviewAsync(reviewId, wrongUserId, updateReviewDto));
        }

        [Fact]
        public async Task ApproveReviewAsync_ValidReview_ShouldApproveReview()
        {
            // Arrange
            var reviewId = 1;
            var review = new Review(1, 1, "user123", 5, "Excelente", DateTime.UtcNow, false);

            _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId))
                .ReturnsAsync(review);
            
            _reviewRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Review>()))
                .ReturnsAsync(review);

            // Act
            var result = await _reviewService.ApproveReviewAsync(reviewId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsApproved);
        }

        [Fact]
        public async Task UserCanReviewProductAsync_NewUser_ShouldReturnTrue()
        {
            // Arrange
            var productId = 1;
            var userId = "newuser";

            _reviewRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Review>());

            // Act
            var result = await _reviewService.UserCanReviewProductAsync(productId, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UserCanReviewProductAsync_UserAlreadyReviewed_ShouldReturnFalse()
        {
            // Arrange
            var productId = 1;
            var userId = "existinguser";
            var existingReview = new Review(productId, userId, 4, "Já avaliei");

            _reviewRepositoryMock.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Review> { existingReview });

            // Act
            var result = await _reviewService.UserCanReviewProductAsync(productId, userId);

            // Assert
            Assert.False(result);
        }
    }
}