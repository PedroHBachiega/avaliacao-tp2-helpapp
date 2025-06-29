using System.Threading.Tasks;
using StockApp.Application.DTOs;

namespace StockApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto> AuthenticateAsync(string username, string password);
        Task<TokenResponseDto> AuthenticateAsync(string username, string password, string provider);
    }
}

