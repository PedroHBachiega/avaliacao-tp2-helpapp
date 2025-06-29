using StockApp.Application.Interfaces;

namespace StockApp.Application.Services
{
    public class TaxService : ITaxService
    {
        private const decimal TaxRate = 0.15m; // 15% de imposto
        public decimal CalculateTax(decimal totalAmount)
        {
            return decimal.Round(totalAmount * TaxRate, 2);
        }
    }
}