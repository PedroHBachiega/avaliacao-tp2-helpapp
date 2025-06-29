using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.API.Models;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Application.Mappings;
using StockApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace StockApp.API.Test
{
    public class ReviewsControllerTests
    {
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<ISentimentAnalysisService> _sentimentAnalysisServiceMock;
        private readonly ReviewsController _controller;
        private readonly IMapper _mapper;

        public ReviewsControllerTests()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _sentimentAnalysisServiceMock = new Mock<ISentimentAnalysisService>();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainToDTOMappingProfile>();
            });
            _mapper = config.CreateMapper();
            
            _controller = new ReviewsController(_reviewServiceMock.Object, _sentimentAnalysisServiceMock.Object);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));
            
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetReview_ExistingReview_ShouldReturnOk()
        {
            // Arrange
            var reviewId = 1;
            var reviewDto = new ReviewDTO
            {
                Id = reviewId,
                ProductId = 1,
                ProductName = "Produto Teste",
                UserId = "user123",
                Rating = 5,
                Comment = "Excelente produto!",
                Date = DateTime.UtcNow,
                IsApproved = true
            };

            _reviewServiceMock.Setup(x => x.GetReviewByIdAsync(reviewId))
                .ReturnsAsync(reviewDto);

            // Act
            var result = await _controller.GetReview(reviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReview = Assert.IsType<ReviewDTO>(okResult.Value);
            Assert.Equal(reviewId, returnedReview.Id);
        }

        [Fact]
        public async Task GetReview_NonExistingReview_ShouldReturnNotFound()
        {
            // Arrange
            var reviewId = 999;
            
            _reviewServiceMock.Setup(x => x.GetReviewByIdAsync(reviewId))
                .ReturnsAsync((ReviewDTO)null);

            // Act
            var result = await _controller.GetReview(reviewId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProductReviews_ValidProductId_ShouldReturnOk()
        {
            // Arrange
            var productId = 1;
            var page = 1;
            var pageSize = 10;
            var reviews = new List<ReviewDTO>
            {
                new ReviewDTO
                {
                    Id = 1,
                    ProductId = productId,
                    ProductName = "Produto Teste",
                    UserId = "user1",
                    Rating = 5,
                    Comment = "Excelente!",
                    Date = DateTime.UtcNow,
                    IsApproved = true
                },
                new ReviewDTO
                {
                    Id = 2,
                    ProductId = productId,
                    ProductName = "Produto Teste",
                    UserId = "user2",
                    Rating = 4,
                    Comment = "Muito bom!",
                    Date = DateTime.UtcNow,
                    IsApproved = true
                }
            };

            _reviewServiceMock.Setup(x => x.GetReviewsByProductIdAsync(productId, page, pageSize))
                .ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetProductReviews(productId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReviews = Assert.IsType<List<ReviewDTO>>(okResult.Value);
            Assert.Equal(2, returnedReviews.Count);
        }

        [Fact]
        public async Task GetProductReviewSummary_ValidProductId_ShouldReturnOk()
        {
            // Arrange
            var productId = 1;
            var summary = new ReviewSummaryDTO
            {
                ProductId = productId,
                AverageRating = 4.5,
                TotalReviews = 10,
                FiveStars = 5,
                FourStars = 3,
                ThreeStars = 1,
                TwoStars = 1,
                OneStar = 0
            };

            _reviewServiceMock.Setup(x => x.GetReviewSummaryByProductIdAsync(productId))
                .ReturnsAsync(summary);

            // Act
            var result = await _controller.GetProductReviewSummary(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedSummary = Assert.IsType<ReviewSummaryDTO>(okResult.Value);
            Assert.Equal(productId, returnedSummary.ProductId);
            Assert.Equal(4.5, returnedSummary.AverageRating);
        }

        [Fact]
        public async Task CreateReview_ValidRequest_ShouldReturnCreated()
        {
            // Arrange
            var createReviewDto = new CreateReviewDTO
            {
                Rating = 5,
                Comment = "Produto excelente!"
            };

            var createdReview = new ReviewDTO
            {
                Id = 1,
                ProductId = 1,
                ProductName = "Produto Teste",
                UserId = "user123",
                Rating = 5,
                Comment = "Produto excelente!",
                Date = DateTime.UtcNow,
                IsApproved = false
            };

            _reviewServiceMock.Setup(x => x.CreateReviewAsync(1, "user123", createReviewDto))
                .ReturnsAsync(createdReview);

            // Act
            var result = await _controller.CreateReview(1, createReviewDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedReview = Assert.IsType<ReviewDTO>(createdResult.Value);
            Assert.Equal(1, returnedReview.Id);
            Assert.Equal(5, returnedReview.Rating);
        }

        [Fact]
        public async Task CreateReview_InvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var createReviewDto = new CreateReviewDTO
            {
                Rating = 0, // Invalid rating
                Comment = "" // Empty comment
            };

            _controller.ModelState.AddModelError("Rating", "Rating must be between 1 and 5");
            _controller.ModelState.AddModelError("Comment", "Comment is required");

            // Act
            var result = await _controller.CreateReview(1, createReviewDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateReview_ValidRequest_ShouldReturnOk()
        {
            // Arrange
            var reviewId = 1;
            var updateReviewDto = new UpdateReviewDTO
            {
                Rating = 4,
                Comment = "Produto bom, mas pode melhorar"
            };

            var updatedReview = new ReviewDTO
            {
                Id = reviewId,
                ProductId = 1,
                ProductName = "Produto Teste",
                UserId = "user123",
                Rating = 4,
                Comment = "Produto bom, mas pode melhorar",
                Date = DateTime.UtcNow,
                IsApproved = false
            };

            _reviewServiceMock.Setup(x => x.UpdateReviewAsync(reviewId, "user123", updateReviewDto))
                .ReturnsAsync(updatedReview);

            // Act
            var result = await _controller.UpdateReview(reviewId, updateReviewDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReview = Assert.IsType<ReviewDTO>(okResult.Value);
            Assert.Equal(4, returnedReview.Rating);
            Assert.Equal("Produto bom, mas pode melhorar", returnedReview.Comment);
        }

        [Fact]
        public async Task DeleteReview_ExistingReview_ShouldReturnNoContent()
        {
            // Arrange
            var reviewId = 1;
            
            _reviewServiceMock.Setup(x => x.DeleteReviewAsync(reviewId, "user123"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteReview(reviewId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteReview_NonExistingReview_ShouldReturnNotFound()
        {
            // Arrange
            var reviewId = 999;
            
            _reviewServiceMock.Setup(x => x.DeleteReviewAsync(reviewId, "user123"))
                .ThrowsAsync(new ArgumentException("Review not found"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _controller.DeleteReview(reviewId));
        }

        [Fact]
        public async Task ApproveReview_ValidReview_ShouldReturnOk()
        {
            // Arrange
            var reviewId = 1;
            var approvedReview = new ReviewModerationDTO
            {
                Id = reviewId,
                ProductId = 1,
                ProductName = "Produto Teste",
                UserId = "user123",
                Rating = 5,
                Comment = "Excelente produto!",
                Date = DateTime.UtcNow,
                IsApproved = true
            };

            _reviewServiceMock.Setup(x => x.ApproveReviewAsync(reviewId))
                .ReturnsAsync(approvedReview);

            // Act
            var result = await _controller.ApproveReview(reviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReview = Assert.IsType<ReviewModerationDTO>(okResult.Value);
            Assert.True(returnedReview.IsApproved);
        }

        [Fact]
        public async Task RejectReview_ValidReview_ShouldReturnOk()
        {
            // Arrange
            var reviewId = 1;
            var rejectedReview = new ReviewModerationDTO
            {
                Id = reviewId,
                ProductId = 1,
                ProductName = "Produto Teste",
                UserId = "user123",
                Rating = 1,
                Comment = "Comentário inadequado",
                Date = DateTime.UtcNow,
                IsApproved = false
            };

            _reviewServiceMock.Setup(x => x.RejectReviewAsync(reviewId))
                .ReturnsAsync(rejectedReview);

            // Act
            var result = await _controller.RejectReview(reviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReview = Assert.IsType<ReviewModerationDTO>(okResult.Value);
            Assert.False(returnedReview.IsApproved);
        }

        [Fact]
        public async Task GetUserReviews_ValidUser_ShouldReturnOk()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var userReviews = new List<ReviewDTO>
            {
                new ReviewDTO
                {
                    Id = 1,
                    ProductId = 1,
                    ProductName = "Produto 1",
                    UserId = "user123",
                    Rating = 5,
                    Comment = "Excelente!",
                    Date = DateTime.UtcNow,
                    IsApproved = true
                },
                new ReviewDTO
                {
                    Id = 2,
                    ProductId = 2,
                    ProductName = "Produto 2",
                    UserId = "user123",
                    Rating = 4,
                    Comment = "Muito bom!",
                    Date = DateTime.UtcNow,
                    IsApproved = true
                }
            };

            _reviewServiceMock.Setup(x => x.GetReviewsByUserIdAsync("user123", page, pageSize))
                .ReturnsAsync(userReviews);

            // Act
            var result = await _controller.GetUserReviews("user123", page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedReviews = Assert.IsType<List<ReviewDTO>>(okResult.Value);
            Assert.Equal(2, returnedReviews.Count);
            Assert.All(returnedReviews, r => Assert.Equal("user123", r.UserId));
        }

        [Fact]
        public async Task AnalyzeSentiment_ValidRequest_ShouldReturnOkWithSentiment()
        {
            // Arrange
            var request = new SentimentAnalysisRequest
            {
                Text = "Excelente produto, estou muito satisfeito!"
            };

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync(request.Text))
                .ReturnsAsync("Positivo");

            // Act
            var result = await _controller.AnalyzeSentiment(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var sentiment = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Positivo", sentiment);
        }

        [Fact]
        public async Task AnalyzeSentiment_EmptyText_ShouldReturnNeutro()
        {
            // Arrange
            var request = new SentimentAnalysisRequest
            {
                Text = ""
            };

            _sentimentAnalysisServiceMock.Setup(x => x.AnalyzeSentimentAsync(request.Text))
                .ReturnsAsync("Neutro");

            // Act
            var result = await _controller.AnalyzeSentiment(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var sentiment = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Neutro", sentiment);
        }

        [Fact]
        public async Task AnalyzeSentiment_NullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.AnalyzeSentiment(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}