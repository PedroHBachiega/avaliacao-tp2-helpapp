using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using StockApp.Infra.Data.Identity;

namespace StockApp.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private static Dictionary<string, ApplicationUser> _users = new Dictionary<string, ApplicationUser>
        {
            { "admin", new ApplicationUser
                {
                    UserName = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin"
                }
            },
            { "user", new ApplicationUser
                {
                    UserName = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = "User"
                }
            }
        };

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<dynamic> GetByUsernameAsync(string username)
        {
            // Implementação básica para exemplo
            // Em um cenário real, você buscaria o usuário no banco de dados
            // Aqui estamos retornando um objeto anônimo para fins de demonstração
            
            // Simula a busca de um usuário no banco de dados
            if (_users.TryGetValue(username.ToLower(), out var user))
            {
                return user;
            }
            
            return null;
        }
        
        public async Task<bool> CreateUserAsync(string username, string passwordHash, string role)
        {
            if (_users.ContainsKey(username.ToLower()))
            {
                return false; // Usuário já existe
            }
            
            _users[username.ToLower()] = new ApplicationUser
            {
                UserName = username,
                PasswordHash = passwordHash,
                Role = role
            };
            
            return true;
        }
    }
}