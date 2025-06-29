using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface ITaxService
    {
        decimal CalculateTax(decimal totalAmount);
    }
}