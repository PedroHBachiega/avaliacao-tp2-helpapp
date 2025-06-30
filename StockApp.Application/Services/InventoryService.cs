using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;

namespace StockApp.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepository;
        public InventoryService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public  async Task ReplenishStockAsync()
        {
            var lowStockProducts = await _productRepository.GetLowStockAsync(10);
            foreach (var product in lowStockProducts)
            {
                product.Stock += 50;
                await _productRepository.Update(product);
            }
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.Create(product);
        }

        public async Task RemoveProductAsync(Product productId)
        {
            await _productRepository.Remove(productId);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.Update(product);
        }
    }
}
