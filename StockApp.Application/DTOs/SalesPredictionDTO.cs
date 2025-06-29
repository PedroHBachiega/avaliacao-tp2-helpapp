namespace StockApp.Application.DTOs
{
    public class SalesPredictionDTO
    {
        public int ProductId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public double PredictedSales { get; set; }
    }
}