namespace StockApp.Application.DTOs
{
    public class SalesReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<DailySalesDTO> DailySales { get; set; } = new();
        public List<CategorySalesDTO> CategorySales { get; set; } = new();
    }

    public class DailySalesDTO
    {
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public int Orders { get; set; }
    }

    public class CategorySalesDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Sales { get; set; }
        public int ProductCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ProductPerformanceReportDTO
    {
        public List<TopProductDTO> TopSellingProducts { get; set; } = new();
        public List<TopProductDTO> LowPerformingProducts { get; set; } = new();
        public List<ProductTrendDTO> ProductTrends { get; set; } = new();
    }

    public class TopProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Sales { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ProductTrendDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<MonthlyDataDTO> MonthlyData { get; set; } = new();
    }

    public class MonthlyDataDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Sales { get; set; }
        public int Units { get; set; }
    }

    public class StockLevelReportDTO
    {
        public List<StockLevelDataDTO> StockLevels { get; set; } = new();
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int OverstockedCount { get; set; }
    }

    public class StockLevelDataDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalStock { get; set; }
        public int LowStock { get; set; }
        public int OutOfStock { get; set; }
        public int Overstocked { get; set; }
    }

    public class RevenueReportDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal GrowthPercentage { get; set; }
        public List<MonthlyRevenueDTO> MonthlyRevenue { get; set; } = new();
        public List<CategoryRevenueDTO> CategoryRevenue { get; set; } = new();
    }

    public class MonthlyRevenueDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public decimal GrowthRate { get; set; }
    }

    public class CategoryRevenueDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public decimal Percentage { get; set; }
        public decimal GrowthRate { get; set; }
    }

    public class DashboardDataDTO
    {
        public DashboardSummaryDTO Summary { get; set; } = new();
        public List<RecentSalesDTO> RecentSales { get; set; } = new();
        public List<TopCategoryDTO> TopCategories { get; set; } = new();
        public List<StockAlertDTO> StockAlerts { get; set; } = new();
    }

    public class DashboardSummaryDTO
    {
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int LowStockItems { get; set; }
        public decimal RevenueGrowth { get; set; }
        public decimal OrderGrowth { get; set; }
    }

    public class RecentSalesDTO
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class TopCategoryDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int ProductCount { get; set; }
    }

    public class StockAlertDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public string AlertType { get; set; } = string.Empty;
    }
}