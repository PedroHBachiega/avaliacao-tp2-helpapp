using StockApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
        Task<PagedResult<ProductDTO>> GetProductsPaged(PaginationParameters paginationParameters);
        
        Task<PagedResult<ProductDTO>> GetProductsWithFiltersAsync(ProductSearchDTO searchParameters);
        
        Task<ProductDTO> GetProductById(int? id);
        Task Add(ProductDTO productDto);
        Task Update(ProductDTO productDto);
        Task Remove(int? id);
        Task<ProductComparisonDTO> CompareProductsAsync(List<int> productIds);
    }
}
