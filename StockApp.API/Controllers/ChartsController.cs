using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;

namespace StockApp.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ChartsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ChartsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("sales")]
        public async Task<ActionResult<SalesReportDTO>> GetSalesChart([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;
            
            var report = await _reportService.GetSalesReportAsync(start, end);
            return Ok(report);
        }

        [HttpGet("products/performance")]
        public async Task<ActionResult<ProductPerformanceReportDTO>> GetProductPerformanceChart([FromQuery] int topCount = 10)
        {
            var report = await _reportService.GetProductPerformanceReportAsync(topCount);
            return Ok(report);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<CategoryReportDTO>> GetCategoriesChart()
        {
            var report = await _reportService.GetCategoryReportAsync();
            return Ok(report);
        }

        [HttpGet("stock-levels")]
        public async Task<ActionResult<StockLevelReportDTO>> GetStockLevelsChart()
        {
            var report = await _reportService.GetStockLevelReportAsync();
            return Ok(report);
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueReportDTO>> GetRevenueChart([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-12);
            var end = endDate ?? DateTime.Today;
            
            var report = await _reportService.GetRevenueReportAsync(start, end);
            return Ok(report);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDataDTO>> GetDashboardData()
        {
            var data = await _reportService.GetDashboardDataAsync();
            return Ok(data);
        }

        [HttpGet("sales/daily")]
        public async Task<ActionResult<object>> GetDailySalesChart([FromQuery] int days = 30)
        {
            var startDate = DateTime.Today.AddDays(-days);
            var endDate = DateTime.Today;
            
            var report = await _reportService.GetSalesReportAsync(startDate, endDate);
            
            var chartData = new
            {
                labels = report.DailySales.Select(d => d.Date.ToString("dd/MM")).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Vendas Diárias",
                        data = report.DailySales.Select(d => d.Sales).ToArray(),
                        backgroundColor = "rgba(54, 162, 235, 0.2)",
                        borderColor = "rgba(54, 162, 235, 1)",
                        borderWidth = 2
                    }
                }
            };
            
            return Ok(chartData);
        }

        [HttpGet("categories/pie")]
        public async Task<ActionResult<object>> GetCategoriesPieChart()
        {
            var report = await _reportService.GetCategoryReportAsync();
            
            var chartData = new
            {
                labels = report.Categories.Select(c => c.CategoryName).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        data = report.Categories.Select(c => c.TotalValue).ToArray(),
                        backgroundColor = new[]
                        {
                            "#FF6384",
                            "#36A2EB",
                            "#FFCE56",
                            "#4BC0C0",
                            "#9966FF",
                            "#FF9F40"
                        }
                    }
                }
            };
            
            return Ok(chartData);
        }

        [HttpGet("stock/bar")]
        public async Task<ActionResult<object>> GetStockBarChart()
        {
            var report = await _reportService.GetStockLevelReportAsync();
            
            var chartData = new
            {
                labels = report.StockLevels.Select(s => s.CategoryName).ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Estoque Total",
                        data = report.StockLevels.Select(s => s.TotalStock).ToArray(),
                        backgroundColor = "rgba(75, 192, 192, 0.6)"
                    },
                    new
                    {
                        label = "Estoque Baixo",
                        data = report.StockLevels.Select(s => s.LowStock).ToArray(),
                        backgroundColor = "rgba(255, 206, 86, 0.6)"
                    },
                    new
                    {
                        label = "Sem Estoque",
                        data = report.StockLevels.Select(s => s.OutOfStock).ToArray(),
                        backgroundColor = "rgba(255, 99, 132, 0.6)"
                    }
                }
            };
            
            return Ok(chartData);
        }

        [HttpGet("revenue/line")]
        public async Task<ActionResult<object>> GetRevenueLineChart([FromQuery] int months = 12)
        {
            var startDate = DateTime.Today.AddMonths(-months);
            var endDate = DateTime.Today;
            
            var report = await _reportService.GetRevenueReportAsync(startDate, endDate);
            
            var chartData = new
            {
                labels = report.MonthlyRevenue.Select(m => $"{m.Month:00}/{m.Year}").ToArray(),
                datasets = new[]
                {
                    new
                    {
                        label = "Receita Mensal",
                        data = report.MonthlyRevenue.Select(m => m.Revenue).ToArray(),
                        borderColor = "rgba(153, 102, 255, 1)",
                        backgroundColor = "rgba(153, 102, 255, 0.2)",
                        fill = true,
                        tension = 0.4
                    }
                }
            };
            
            return Ok(chartData);
        }
    }
}