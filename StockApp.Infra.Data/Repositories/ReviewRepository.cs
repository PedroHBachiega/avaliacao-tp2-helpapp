using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Infra.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Review> reviews, int totalCount)> GetReviewsPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.Date);

            var totalCount = await query.CountAsync();
            var reviews = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (reviews, totalCount);
        }

        public async Task<(IEnumerable<Review> reviews, int totalCount)> GetProductReviewsPagedAsync(int productId, int pageNumber, int pageSize)
        {
            var query = _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.Date);

            var totalCount = await query.CountAsync();
            var reviews = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (reviews, totalCount);
        }

        public async Task<(IEnumerable<Review> reviews, int totalCount)> GetUserReviewsPagedAsync(string userId, int pageNumber, int pageSize)
        {
            var totalCount = await _context.Reviews
                .CountAsync(r => r.UserId == userId);

            var reviews = await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (reviews, totalCount);
        }

        public async Task<Review> CreateAsync(Review review)
        {
            _context.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            _context.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> RemoveAsync(Review review)
        {
            _context.Remove(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<double> GetAverageRatingByProductIdAsync(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId && r.IsApproved)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<int> GetReviewCountByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .CountAsync(r => r.ProductId == productId && r.IsApproved);
        }

        public async Task<IEnumerable<Review>> GetPendingReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => !r.IsApproved)
                .OrderBy(r => r.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetApprovedReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.ProductId == productId && r.IsApproved)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }
    }
}