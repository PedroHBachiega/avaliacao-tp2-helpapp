using StockApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetByUserIdAsync(string userId);
        Task<(IEnumerable<Review> reviews, int totalCount)> GetReviewsPagedAsync(int pageNumber, int pageSize);
        Task<(IEnumerable<Review> reviews, int totalCount)> GetProductReviewsPagedAsync(int productId, int pageNumber, int pageSize);
        Task<(IEnumerable<Review> reviews, int totalCount)> GetUserReviewsPagedAsync(string userId, int pageNumber, int pageSize);
        Task<Review> CreateAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task<Review> RemoveAsync(Review review);
        Task<double> GetAverageRatingByProductIdAsync(int productId);
        Task<int> GetReviewCountByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetPendingReviewsAsync();
        Task<IEnumerable<Review>> GetApprovedReviewsByProductIdAsync(int productId);
    }
}