using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using StockApp.Application.DTOs;

namespace StockApp.API.Test
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterAndLogin_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto
            {
                Username = "testuser",
                Password = "password",
                Role = "User"
            };

            var userLoginDto = new UserLoginDTO
            {
                Username = "testuser",
                Password = "password"
            };

            //Register//
            var registerResponse = await _client.PostAsJsonAsync("/api/users/register", userRegisterDto);
            registerResponse.EnsureSuccessStatusCode();

            //Login//
            var loginResponse = await _client.PostAsJsonAsync("/api/users/login", userLoginDto);
            loginResponse.EnsureSuccessStatusCode();

            var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

            //Assert//
            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrEmpty(tokenResponse?.Token));
        }
    }
}
