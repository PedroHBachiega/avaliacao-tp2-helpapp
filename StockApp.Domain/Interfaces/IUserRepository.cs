using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<dynamic> GetByUsernameAsync(string username);
    }
}