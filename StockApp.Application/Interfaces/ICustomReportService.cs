using StockApp.Application.DTOs;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface ICustomReportService
    {
        Task<CustomReportDto> GenerateReportAsync(ReportParametersDto parameters);
    }
}