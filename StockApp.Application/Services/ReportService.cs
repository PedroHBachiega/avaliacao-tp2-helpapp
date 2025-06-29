using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ReportService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<SalesReportDTO> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            var products = await _productRepository.GetProducts();
            var categories = await _categoryRepository.GetCategories();
            
            var dailySales = new List<DailySalesDTO>();
            var random = new Random();
            
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dailySales.Add(new DailySalesDTO
                {
                    Date = date,
                    Sales = random.Next(1000, 5000),
                    Orders = random.Next(10, 50)
                });
            }

            var categorySales = categories.Select(c => new CategorySalesDTO
            {
                CategoryName = c.Name,
                Sales = random.Next(5000, 20000),
                ProductCount = products.Count(p => p.CategoryId == c.Id),
                Percentage = random.Next(10, 30)
            }).ToList();

            var totalSales = dailySales.Sum(d => d.Sales);
            var totalOrders = dailySales.Sum(d => d.Orders);

            return new SalesReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = totalSales,
                TotalOrders = totalOrders,
                AverageOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0,
                DailySales = dailySales,
                CategorySales = categorySales
            };
        }

        public async Task<ProductPerformanceReportDTO> GetProductPerformanceReportAsync(int topCount = 10)
        {
            var products = await _productRepository.GetProducts();
            var random = new Random();

            var topProducts = products.Take(topCount).Select(p => new TopProductDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Sales = random.Next(100, 1000),
                UnitsSold = random.Next(50, 500),
                Revenue = p.Price * random.Next(50, 500)
            }).ToList();

            var lowProducts = products.Skip(topCount).Take(5).Select(p => new TopProductDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Sales = random.Next(10, 100),
                UnitsSold = random.Next(1, 50),
                Revenue = p.Price * random.Next(1, 50)
            }).ToList();

            var productTrends = products.Take(5).Select(p => new ProductTrendDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                MonthlyData = Enumerable.Range(1, 12).Select(month => new MonthlyDataDTO
                {
                    Month = month,
                    Year = DateTime.Now.Year,
                    Sales = random.Next(100, 1000),
                    Units = random.Next(10, 100)
                }).ToList()
            }).ToList();

            return new ProductPerformanceReportDTO
            {
                TopSellingProducts = topProducts,
                LowPerformingProducts = lowProducts,
                ProductTrends = productTrends
            };
        }

        public async Task<CategoryReportDTO> GetCategoryReportAsync()
        {
            var categories = await _categoryRepository.GetCategories();
            var products = await _productRepository.GetProducts();
            var random = new Random();

            var categoryStats = categories.Select(c => new CategoryStatsDTO
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                ProductCount = products.Count(p => p.CategoryId == c.Id),
                TotalValue = products.Where(p => p.CategoryId == c.Id).Sum(p => p.Price * p.Stock),
                AveragePrice = products.Where(p => p.CategoryId == c.Id).Any() ? 
                    products.Where(p => p.CategoryId == c.Id).Average(p => p.Price) : 0,
                TotalStock = products.Where(p => p.CategoryId == c.Id).Sum(p => p.Stock),
                PercentageOfTotal = random.Next(5, 25)
            }).ToList();

            return new CategoryReportDTO
            {
                Title = "Relatório de Categorias",
                GeneratedAt = DateTime.UtcNow,
                Summary = new CategoryReportSummaryDTO
                {
                    TotalCategories = categories.Count(),
                    TotalProducts = products.Count(),
                    TotalValue = categoryStats.Sum(c => c.TotalValue)
                },
                Categories = categoryStats
            };
        }

        public async Task<StockLevelReportDTO> GetStockLevelReportAsync()
        {
            var categories = await _categoryRepository.GetCategories();
            var products = await _productRepository.GetProducts();
            var random = new Random();

            var stockLevels = categories.Select(c => new StockLevelDataDTO
            {
                CategoryName = c.Name,
                TotalStock = products.Where(p => p.CategoryId == c.Id).Sum(p => p.Stock),
                LowStock = random.Next(1, 5),
                OutOfStock = random.Next(0, 3),
                Overstocked = random.Next(0, 2)
            }).ToList();

            return new StockLevelReportDTO
            {
                StockLevels = stockLevels,
                TotalProducts = products.Count(),
                LowStockCount = stockLevels.Sum(s => s.LowStock),
                OutOfStockCount = stockLevels.Sum(s => s.OutOfStock),
                OverstockedCount = stockLevels.Sum(s => s.Overstocked)
            };
        }

        public async Task<RevenueReportDTO> GetRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var random = new Random();
            
            var monthlyRevenue = new List<MonthlyRevenueDTO>();
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);
            var endDateMonth = new DateTime(endDate.Year, endDate.Month, 1);

            while (currentDate <= endDateMonth)
            {
                var revenue = random.Next(10000, 50000);
                monthlyRevenue.Add(new MonthlyRevenueDTO
                {
                    Month = currentDate.Month,
                    Year = currentDate.Year,
                    Revenue = revenue,
                    GrowthRate = random.Next(-10, 20)
                });
                currentDate = currentDate.AddMonths(1);
            }

            var categories = await _categoryRepository.GetCategories();
            var categoryRevenue = categories.Select(c => new CategoryRevenueDTO
            {
                CategoryName = c.Name,
                Revenue = random.Next(5000, 25000),
                Percentage = random.Next(10, 30),
                GrowthRate = random.Next(-5, 15)
            }).ToList();

            var totalRevenue = monthlyRevenue.Sum(m => m.Revenue);

            return new RevenueReportDTO
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = totalRevenue,
                GrowthPercentage = random.Next(5, 20),
                MonthlyRevenue = monthlyRevenue,
                CategoryRevenue = categoryRevenue
            };
        }

        public async Task<DashboardDataDTO> GetDashboardDataAsync()
        {
            var products = await _productRepository.GetProducts();
            var categories = await _categoryRepository.GetCategories();
            var random = new Random();

            var recentSales = Enumerable.Range(0, 7).Select(i => new RecentSalesDTO
            {
                Date = DateTime.Today.AddDays(-i),
                Amount = random.Next(1000, 5000)
            }).OrderBy(r => r.Date).ToList();

            var topCategories = categories.Take(5).Select(c => new TopCategoryDTO
            {
                Name = c.Name,
                Revenue = random.Next(5000, 20000),
                ProductCount = products.Count(p => p.CategoryId == c.Id)
            }).ToList();

            var stockAlerts = products.Where(p => p.Stock < 10).Take(5).Select(p => new StockAlertDTO
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CurrentStock = p.Stock,
                MinimumStock = 10,
                AlertType = p.Stock == 0 ? "Sem Estoque" : "Estoque Baixo"
            }).ToList();

            return new DashboardDataDTO
            {
                Summary = new DashboardSummaryDTO
                {
                    TotalRevenue = random.Next(50000, 100000),
                    TotalProducts = products.Count(),
                    TotalOrders = random.Next(100, 500),
                    LowStockItems = stockAlerts.Count,
                    RevenueGrowth = random.Next(5, 20),
                    OrderGrowth = random.Next(2, 15)
                },
                RecentSales = recentSales,
                TopCategories = topCategories,
                StockAlerts = stockAlerts
            };
        }
    }
}