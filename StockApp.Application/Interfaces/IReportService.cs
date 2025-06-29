using StockApp.Application.DTOs;

namespace StockApp.Application.Interfaces
{
    public interface IReportService
    {
        Task<SalesReportDTO> GetSalesReportAsync(DateTime startDate, DateTime endDate);
        Task<ProductPerformanceReportDTO> GetProductPerformanceReportAsync(int topCount = 10);
        Task<CategoryReportDTO> GetCategoryReportAsync();
        Task<StockLevelReportDTO> GetStockLevelReportAsync();
        Task<RevenueReportDTO> GetRevenueReportAsync(DateTime startDate, DateTime endDate);
        Task<DashboardDataDTO> GetDashboardDataAsync();
    }
}