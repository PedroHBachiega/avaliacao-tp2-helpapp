using StockApp.Application.Interfaces;
namespace StockApp.Application.Services
{
    public class DiscountService : IDiscountService
    {
        public decimal ApplyDiscount(decimal price, decimal? discountPercentage)
        {
            if (discountPercentage == null || discountPercentage <= 0) return price;
            return price - (price * discountPercentage.Value / 100);
        }
    }
}