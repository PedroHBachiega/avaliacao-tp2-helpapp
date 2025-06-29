using StockApp.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();
        Task<ReviewDTO> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<ReviewDTO>> GetReviewsByProductIdAsync(int productId, int pageNumber, int pageSize);
        Task<IEnumerable<ReviewDTO>> GetReviewsByUserIdAsync(string userId);
        Task<IEnumerable<ReviewDTO>> GetReviewsByUserIdAsync(string userId, int pageNumber, int pageSize);
        Task<PagedResult<ReviewDTO>> GetReviewsPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<ReviewDTO>> GetProductReviewsPagedAsync(int productId, int pageNumber, int pageSize);
        Task<ReviewDTO> CreateReviewAsync(int productId, string userId, CreateReviewDTO createReviewDto);
        Task<ReviewDTO> UpdateReviewAsync(int id, string userId, UpdateReviewDTO updateReviewDto);
        Task DeleteReviewAsync(int id, string userId);
        Task<ReviewSummaryDTO> GetReviewSummaryByProductIdAsync(int productId);
        Task<IEnumerable<ReviewDTO>> GetPendingReviewsAsync();
        Task<ReviewModerationDTO> ApproveReviewAsync(int id);
        Task<ReviewModerationDTO> RejectReviewAsync(int id);
        Task<bool> UserCanReviewProductAsync(int productId, string userId);
    }
}