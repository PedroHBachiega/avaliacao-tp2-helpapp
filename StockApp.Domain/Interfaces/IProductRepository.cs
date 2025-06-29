using StockApp.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace StockApp.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<(IEnumerable<Product> products, int totalCount)> GetProductsPaged(int pageNumber, int pageSize);
        
        Task<(IEnumerable<Product> products, int totalCount)> GetProductsWithFiltersAsync(
            int pageNumber, 
            int pageSize,
            string? name = null,
            string? description = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int? minStock = null,
            int? maxStock = null,
            bool? isLowStock = null,
            string? sortBy = null,
            string? sortDirection = "asc");

        Task<IEnumerable<Product>> GetAllAsync();

        Task<Product> GetById(int? id);
        Task<Product> Create(Product product);
        Task<Product> Update(Product product);
        Task<Product> Remove(Product product);
        Task<IEnumerable<Product>> GetLowStockAsync(int threshold);
        Task UpdateAsync(Product product);

    }
}
