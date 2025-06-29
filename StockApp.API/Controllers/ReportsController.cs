using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace StockApp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ReportsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<object>> GetProductsReport([FromQuery] ProductSearchDTO searchParameters)
        {
            if (!searchParameters.IsValid())
            {
                var errors = searchParameters.GetValidationErrors();
                return BadRequest(new { errors });
            }

            var filteredProducts = await _productService.GetProductsWithFiltersAsync(searchParameters);

            var totalValue = filteredProducts.Data.Sum(p => p.Price * p.Stock);
            var averagePrice = filteredProducts.Data.Any() ? filteredProducts.Data.Average(p => p.Price) : 0;
            var lowStockCount = filteredProducts.Data.Count(p => p.Stock < 10);
            var totalProducts = filteredProducts.TotalRecords;

            var report = new
            {
                Summary = new
                {
                    TotalProducts = totalProducts,
                    TotalValue = totalValue,
                    AveragePrice = Math.Round(averagePrice, 2),
                    LowStockCount = lowStockCount,
                    FilteredResults = filteredProducts.Data.Count(),
                    GeneratedAt = DateTime.UtcNow
                },
                Filters = searchParameters.Filters,
                Pagination = new
                {
                    CurrentPage = filteredProducts.CurrentPage,
                    PageSize = filteredProducts.PageSize,
                    TotalPages = filteredProducts.TotalPages,
                    TotalCount = filteredProducts.TotalRecords
                },
                Products = filteredProducts.Data
            };

            return Ok(report);
        }

        [HttpPost("products")]
        public async Task<ActionResult<object>> GetProductsReportPost([FromBody] ProductSearchDTO searchParameters)
        {
            return await GetProductsReport(searchParameters);
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<object>> GetLowStockReport([FromQuery] int threshold = 10)
        {
            var searchParameters = new ProductSearchDTO
            {
                PageNumber = 1,
                PageSize = 1000,
                Filters = new ProductFilterDTO
                {
                    MaxStock = threshold - 1,
                    SortBy = "stock",
                    SortDirection = "asc"
                }
            };

            var lowStockProducts = await _productService.GetProductsWithFiltersAsync(searchParameters);

            var report = new
            {
                Summary = new
                {
                    Threshold = threshold,
                    TotalLowStockProducts = lowStockProducts.TotalRecords,
                    TotalValue = lowStockProducts.Data.Sum(p => p.Price * p.Stock),
                    GeneratedAt = DateTime.UtcNow
                },
                Products = lowStockProducts.Data.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Stock,
                    p.Price,
                    Value = p.Price * p.Stock,
                    Category = p.Category?.Name,
                    StockStatus = p.Stock == 0 ? "Out of Stock" : "Low Stock"
                })
            };

            return Ok(report);
        }

        [HttpGet("by-category")]
        public async Task<ActionResult<object>> GetProductsByCategoryReport()
        {
            var allProducts = await _productService.GetProducts();
            var categories = await _categoryService.GetCategories();

            var categoryReport = categories.Select(category => new
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                ProductCount = allProducts.Count(p => p.CategoryId == category.Id),
                TotalValue = allProducts.Where(p => p.CategoryId == category.Id).Sum(p => p.Price * p.Stock),
                AveragePrice = allProducts.Where(p => p.CategoryId == category.Id).Any() 
                    ? allProducts.Where(p => p.CategoryId == category.Id).Average(p => p.Price) 
                    : 0,
                TotalStock = allProducts.Where(p => p.CategoryId == category.Id).Sum(p => p.Stock)
            }).OrderByDescending(c => c.TotalValue);

            var report = new
            {
                Summary = new
                {
                    TotalCategories = categories.Count(),
                    TotalProducts = allProducts.Count(),
                    TotalValue = allProducts.Sum(p => p.Price * p.Stock),
                    GeneratedAt = DateTime.UtcNow
                },
                Categories = categoryReport
            };

            return Ok(report);
        }
    }
}
