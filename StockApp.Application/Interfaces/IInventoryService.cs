using StockApp.Domain.Entities;

namespace StockApp.Application.Interfaces
{
    public interface IInventoryService
    {
        Task ReplenishStockAsync();
        Task AddProductAsync(Product product);
        Task RemoveProductAsync(Product productId);
        Task UpdateProductAsync(Product product);
    }
}
