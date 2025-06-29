using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Models;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        public ReviewsController(IReviewService reviewService, ISentimentAnalysisService sentimentAnalysisService)
        {
            _reviewService = reviewService;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<ReviewDTO>>> GetReviews([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reviewService.GetReviewsPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<PagedResult<ReviewDTO>>> GetProductReviews(int productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _reviewService.GetProductReviewsPagedAsync(productId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("product/{productId}/summary")]
        public async Task<ActionResult<ReviewSummaryDTO>> GetProductReviewSummary(int productId)
        {
            var summary = await _reviewService.GetReviewSummaryByProductIdAsync(productId);
            return Ok(summary);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetUserReviews(string userId, int page = 1, int pageSize = 10)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var reviews = await _reviewService.GetReviewsByUserIdAsync(userId, page, pageSize);
            return Ok(reviews);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetPendingReviews()
        {
            var reviews = await _reviewService.GetPendingReviewsAsync();
            return Ok(reviews);
        }

        [HttpPost("product/{productId}")]
        [Authorize]
        public async Task<ActionResult<ReviewDTO>> CreateReview(int productId, [FromBody] CreateReviewDTO createReviewDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var review = await _reviewService.CreateReviewAsync(productId, userId, createReviewDto);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReviewDTO>> UpdateReview(int id, [FromBody] UpdateReviewDTO updateReviewDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var review = await _reviewService.UpdateReviewAsync(id, userId, updateReviewDto);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                await _reviewService.DeleteReviewAsync(id, userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ReviewDTO>> ApproveReview(int id)
        {
            try
            {
                var review = await _reviewService.ApproveReviewAsync(id);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ReviewDTO>> RejectReview(int id)
        {
            try
            {
                var review = await _reviewService.RejectReviewAsync(id);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Analisa o sentimento de um texto
        /// </summary>
        /// <param name="text">Texto a ser analisado</param>
        /// <returns>Resultado da análise de sentimento (Positivo, Negativo, Neutro)</returns>
        [HttpPost("analyze-sentiment")]
        public async Task<ActionResult<string>> AnalyzeSentiment([FromBody] SentimentAnalysisRequest request)
        {
            if (request == null)
                return BadRequest();
                
            if (string.IsNullOrWhiteSpace(request.Text))
                return Ok("Neutro");

            var sentiment = await _sentimentAnalysisService.AnalyzeSentimentAsync(request.Text);
            return Ok(sentiment);
        }
    }
}