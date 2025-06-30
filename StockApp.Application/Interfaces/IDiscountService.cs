using System;
namespace StockApp.Application.Interfaces
{
    public interface IDiscountService
    {
        decimal ApplyDiscount(decimal price, decimal? discountPercentage);
    }
}