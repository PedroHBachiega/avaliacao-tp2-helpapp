using System.Threading.Tasks;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using BCrypt.Net;

namespace StockApp.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(UserRegisterDto userDto)
        {
            // 1. Validações básicas
            if (string.IsNullOrWhiteSpace(userDto.Username) ||
                string.IsNullOrWhiteSpace(userDto.Password) ||
                string.IsNullOrWhiteSpace(userDto.Role))
            {
                return false;
            }
                
            if (userDto.Username.Length < 3 || userDto.Username.Length > 20)
                return false;
            
            if (userDto.Password.Length < 8)
                return false;
                
            // Após validar, criar o usuário
            return await CreateUserAsync(userDto);
        }

        public async Task<dynamic> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByUsernameAsync(email);
        }

        public async Task<bool> CreateUserAsync(UserRegisterDto user)
        {
            try
            {
                // Verificar se o usuário já existe
                var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
                if (existingUser != null)
                {
                    return false;
                }

                // Hash da senha
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

                // Criar usuário usando o método do repositório
                return await _userRepository.CreateUserAsync(user.Username, hashedPassword, user.Role);
            }
            catch
            {
                return false;
            }
        }
    }
}