using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using BCrypt.Net;

namespace StockApp.Infra.Data.Identity
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<TokenResponseDto> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        public async Task<TokenResponseDto> AuthenticateAsync(string username, string password, string provider)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }

            // Para autenticação social, não verificamos a senha
            if (!string.IsNullOrEmpty(provider))
            {
                return GenerateJwtToken(user);
            }

            // Para autenticação tradicional, verificamos a senha
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return GenerateJwtToken(user);
        }

        private TokenResponseDto GenerateJwtToken(dynamic user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            
            // Verifica se a propriedade é Username ou UserName
            string username = null;
            if (user.GetType().GetProperty("Username") != null)
            {
                username = user.Username;
            }
            else if (user.GetType().GetProperty("UserName") != null)
            {
                username = user.UserName;
            }
            else
            {
                // Fallback para objetos dinâmicos
                try { username = user.Username; } catch { }
                if (username == null) try { username = user.UserName; } catch { }
                if (username == null) username = "unknown";
            }
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("Permission", "CanManageStock")
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = token.ValidTo
            };
        }
    }
}