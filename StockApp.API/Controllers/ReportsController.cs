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
        private readonly ITaxService _taxService;

        public ReportsController(IProductService productService, ICategoryService categoryService, ITaxService taxService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _taxService = taxService;
        }

        [HttpGet("tax-report")]
        public async Task<IActionResult> GetTaxReport()
        {
            var products = await _productService.GetProducts();
            var totalSales = products.Sum(p => p.Price * p.Stock); // Exemplo: total de vendas
            var taxAmount = _taxService.CalculateTax(totalSales);
            return Ok(new { TotalSales = totalSales, TaxAmount = taxAmount, TaxRate = "15%" });
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

        [HttpGet("products/export/csv")]
        public async Task<IActionResult> ExportProductsToCSV([FromQuery] ProductSearchDTO searchParameters)
        {
            if (!searchParameters.IsValid())
            {
                var errors = searchParameters.GetValidationErrors();
                return BadRequest(new { errors });
            }

            searchParameters.PageSize = int.MaxValue;
            searchParameters.PageNumber = 1;
            
            var filteredProducts = await _productService.GetProductsWithFiltersAsync(searchParameters);
            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,Description,Price,Stock,Category,TotalValue");

            foreach (var product in filteredProducts.Data)
            {
                csv.AppendLine($"{product.Id},\"{product.Name}\",\"{product.Description}\",{product.Price},{product.Stock},\"{product.Category?.Name}\",{product.Price * product.Stock}");
            }

            var fileName = $"products_report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(bytes, "text/csv", fileName);
        }
    }
}
