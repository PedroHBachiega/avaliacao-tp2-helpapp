﻿using System.Threading.Tasks;
using StockApp.Application.DTOs;

namespace StockApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(UserRegisterDto user);
    }

    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Role { get; set; }
       
    }
}
