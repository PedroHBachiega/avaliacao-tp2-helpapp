using AutoMapper;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace StockApp.Application.Services
{
    public class ProductService : IProductService
    {
        private IProductRepository _productRepository;
        private IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task Add(ProductDTO productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            await _productRepository.Create(productEntity);
        }

        public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var productsEntity = await _productRepository.GetProducts();
            return _mapper.Map<IEnumerable<ProductDTO>>(productsEntity);
        }

        public async Task<PagedResult<ProductDTO>> GetProductsPaged(PaginationParameters paginationParameters)
        {
            var (products, totalCount) = await _productRepository.GetProductsPaged(
                paginationParameters.PageNumber,
                paginationParameters.PageSize);

            var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return new PagedResult<ProductDTO>(
                productDTOs,
                paginationParameters.PageNumber,
                paginationParameters.PageSize,
                totalCount);
        }

        public async Task<PagedResult<ProductDTO>> GetProductsWithFiltersAsync(ProductSearchDTO searchParameters)
        {
            var filters = searchParameters.Filters;
            
            var (products, totalCount) = await _productRepository.GetProductsWithFiltersAsync(
                searchParameters.PageNumber,
                searchParameters.PageSize,
                filters.Name,
                filters.Description,
                filters.CategoryId,
                filters.MinPrice,
                filters.MaxPrice,
                filters.MinStock,
                filters.MaxStock,
                filters.IsLowStock,
                filters.SortBy,
                filters.SortDirection);
            
            var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(products);
            
            return new PagedResult<ProductDTO>(
                productDTOs, 
                searchParameters.PageNumber, 
                searchParameters.PageSize, 
                totalCount);
        }

        public async Task<PagedResult<ProductDTO>> AdvancedSearchAsync(AdvancedSearchDTO searchParameters)
        {
            if (!searchParameters.IsValid())
            {
                throw new ArgumentException("Invalid search parameters: " + string.Join(", ", searchParameters.GetValidationErrors()));
            }

            var (products, totalCount) = await _productRepository.AdvancedSearchAsync(
                searchParameters.PageNumber,
                searchParameters.PageSize,
                searchParameters.SearchTerm,
                searchParameters.Name,
                searchParameters.Description,
                searchParameters.CategoryId,
                searchParameters.CategoryIds,
                searchParameters.MinPrice,
                searchParameters.MaxPrice,
                searchParameters.MinStock,
                searchParameters.MaxStock,
                searchParameters.IsLowStock,
                searchParameters.HasPromotion,
                searchParameters.MinDiscountPercentage,
                searchParameters.SortBy,
                searchParameters.SortDirection,
                searchParameters.SecondarySortBy,
                searchParameters.SecondarySortDirection,
                searchParameters.IncludeWithoutCategory,
                searchParameters.ExactMatch,
                searchParameters.CaseSensitive);

            var productDTOs = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return new PagedResult<ProductDTO>(
                productDTOs,
                searchParameters.PageNumber,
                searchParameters.PageSize,
                totalCount);
        }

        public async Task<ProductDTO> GetProductById(int? id)
        {
            var productEntity = _productRepository.GetById(id);
            return _mapper.Map<ProductDTO>(productEntity);
        }

        public async Task Remove(int? id)
        {
            var productEntity = _productRepository.GetById(id).Result;
            await _productRepository.Remove(productEntity);
        }

        public async Task Update(ProductDTO productDto)
        {
            var productEntity = _mapper.Map<Product>(productDto);
            await _productRepository.Update(productEntity);
        }

        public async Task<ProductComparisonDTO> CompareProductsAsync(List<int> productIds)
        {
            if (productIds == null || productIds.Count < 2)
                throw new ArgumentException("É necessário pelo menos 2 produtos para comparação");

            if (productIds.Count > 10)
                throw new ArgumentException("Máximo de 10 produtos podem ser comparados por vez");

            var products = new List<ProductDTO>();
            
            foreach (var id in productIds)
            {
                var product = await GetProductById(id);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            if (products.Count < 2)
                throw new ArgumentException("Produtos não encontrados ou insuficientes para comparação");

            var summary = new ProductComparisonSummaryDTO
            {
                TotalProductsCompared = products.Count,
                HighestPrice = products.Max(p => p.Price),
                LowestPrice = products.Min(p => p.Price),
                AveragePrice = products.Average(p => p.Price),
                HighestStock = products.Max(p => p.Stock),
                LowestStock = products.Min(p => p.Stock),
                AverageStock = products.Average(p => p.Stock),
                MostExpensive = products.OrderByDescending(p => p.Price).First(),
                Cheapest = products.OrderBy(p => p.Price).First(),
                HighestStockProduct = products.OrderByDescending(p => p.Stock).First(),
                LowestStockProduct = products.OrderBy(p => p.Stock).First()
            };

            summary.PriceDifference = summary.HighestPrice - summary.LowestPrice;
            summary.StockDifference = summary.HighestStock - summary.LowestStock;

            var categoryGroups = products.GroupBy(p => p.Category?.Name)
                .Where(g => g.Key != null && g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            
            summary.CommonCategories = categoryGroups;

            return new ProductComparisonDTO
            {
                Products = products,
                Summary = summary,
                ComparedAt = DateTime.UtcNow
            };
        }
    }
}
