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
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository, IMapper mapper, ISentimentAnalysisService sentimentAnalysisService)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews).ToList();
            
            // Adiciona análise de sentimento para cada review
            for (int i = 0; i < reviewDtos.Count; i++)
            {
                reviewDtos[i].SentimentAnalysis = await _sentimentAnalysisService.AnalyzeSentimentAsync(reviewDtos[i].Comment);
            }
            
            return reviewDtos;
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            var reviewDto = _mapper.Map<ReviewDTO>(review);
            
            if (reviewDto != null)
            {
                reviewDto.SentimentAnalysis = await _sentimentAnalysisService.AnalyzeSentimentAsync(reviewDto.Comment);
            }
            
            return reviewDto;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepository.GetApprovedReviewsByProductIdAsync(productId);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews).ToList();
            
            // Adiciona análise de sentimento para cada review
            for (int i = 0; i < reviewDtos.Count; i++)
            {
                reviewDtos[i].SentimentAnalysis = await _sentimentAnalysisService.AnalyzeSentimentAsync(reviewDtos[i].Comment);
            }
            
            return reviewDtos;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId, int pageNumber, int pageSize)
        {
            var (reviews, _) = await _reviewRepository.GetProductReviewsPagedAsync(productId, pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByUserIdAsync(string userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByUserIdAsync(string userId, int pageNumber, int pageSize)
        {
            var (reviews, _) = await _reviewRepository.GetUserReviewsPagedAsync(userId, pageNumber, pageSize);
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<PagedResult<ReviewDTO>> GetReviewsPagedAsync(int pageNumber, int pageSize)
        {
            var (reviews, totalCount) = await _reviewRepository.GetReviewsPagedAsync(pageNumber, pageSize);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);

            return new PagedResult<ReviewDTO>
            {
                Data = reviewDtos,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<ReviewDTO>> GetProductReviewsPagedAsync(int productId, int pageNumber, int pageSize)
        {
            var (reviews, totalCount) = await _reviewRepository.GetProductReviewsPagedAsync(productId, pageNumber, pageSize);
            var reviewDtos = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);

            return new PagedResult<ReviewDTO>
            {
                Data = reviewDtos,
                TotalRecords = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ReviewDTO> CreateReviewAsync(int productId, string userId, CreateReviewDTO createReviewDto)
        {
            var product = await _productRepository.GetById(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var canReview = await UserCanReviewProductAsync(productId, userId);
            if (!canReview)
                throw new InvalidOperationException("User has already reviewed this product");

            var review = new Review(productId, userId, createReviewDto.Rating, createReviewDto.Comment);
            var createdReview = await _reviewRepository.CreateAsync(review);

            var reviewDto = _mapper.Map<ReviewDTO>(createdReview);
            reviewDto.SentimentAnalysis = await _sentimentAnalysisService.AnalyzeSentimentAsync(reviewDto.Comment);
            
            return reviewDto;
        }

        public async Task<ReviewDTO> UpdateReviewAsync(int id, string userId, UpdateReviewDTO updateReviewDto)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new ArgumentException("Review not found");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("User can only update their own reviews");

            review.Update(updateReviewDto.Rating, updateReviewDto.Comment);
            var updatedReview = await _reviewRepository.UpdateAsync(review);

            var reviewDto = _mapper.Map<ReviewDTO>(updatedReview);
            reviewDto.SentimentAnalysis = await _sentimentAnalysisService.AnalyzeSentimentAsync(reviewDto.Comment);
            
            return reviewDto;
        }

        public async Task DeleteReviewAsync(int id, string userId)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new ArgumentException("Review not found");

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("User can only delete their own reviews");

            await _reviewRepository.RemoveAsync(review);
        }

        public async Task<ReviewSummaryDTO> GetReviewSummaryByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepository.GetApprovedReviewsByProductIdAsync(productId);
            var reviewsList = reviews.ToList();

            if (!reviewsList.Any())
            {
                return new ReviewSummaryDTO
                {
                    ProductId = productId,
                    AverageRating = 0,
                    TotalReviews = 0
                };
            }

            return new ReviewSummaryDTO
            {
                ProductId = productId,
                AverageRating = reviewsList.Average(r => r.Rating),
                TotalReviews = reviewsList.Count,
                FiveStars = reviewsList.Count(r => r.Rating == 5),
                FourStars = reviewsList.Count(r => r.Rating == 4),
                ThreeStars = reviewsList.Count(r => r.Rating == 3),
                TwoStars = reviewsList.Count(r => r.Rating == 2),
                OneStar = reviewsList.Count(r => r.Rating == 1)
            };
        }

        public async Task<IEnumerable<ReviewDTO>> GetPendingReviewsAsync()
        {
            var reviews = await _reviewRepository.GetPendingReviewsAsync();
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<ReviewModerationDTO> ApproveReviewAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new ArgumentException("Review not found");

            review.Approve();
            var updatedReview = await _reviewRepository.UpdateAsync(review);

            return _mapper.Map<ReviewModerationDTO>(updatedReview);
        }

        public async Task<ReviewModerationDTO> RejectReviewAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new ArgumentException("Review not found");

            review.Reject();
            var updatedReview = await _reviewRepository.UpdateAsync(review);

            return _mapper.Map<ReviewModerationDTO>(updatedReview);
        }

        public async Task<bool> UserCanReviewProductAsync(int productId, string userId)
        {
            var existingReviews = await _reviewRepository.GetByUserIdAsync(userId);
            return !existingReviews.Any(r => r.ProductId == productId);
        }
    }
}