using System.Collections.Generic;

namespace StockApp.Application.DTOs
{
    public class ReportParametersDto
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public int? ProductId { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public Dictionary<string, string>? AdditionalParameters { get; set; }
    }

    public class ReportDataDto
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class CustomReportDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public List<ReportDataDto> Data { get; set; } = new List<ReportDataDto>();
    }
}