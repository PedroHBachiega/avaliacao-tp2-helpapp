using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using StockApp.API.Infrastructure.Middlewares;
using System.IO;
using System.Text;
using Xunit;

namespace StockApp.API.Test
{
    public class RateLimitingMiddlewareTests
    {
        private readonly Mock<ILogger<RateLimitingMiddleware>> _loggerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly RateLimitOptions _options;

        public RateLimitingMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<RateLimitingMiddleware>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _options = new RateLimitOptions
            {
                DefaultPolicy = new RateLimitPolicy
                {
                    MaxRequests = 5,
                    Window = TimeSpan.FromMinutes(1)
                },
                WriteOperationsPolicy = new RateLimitPolicy
                {
                    MaxRequests = 3,
                    Window = TimeSpan.FromMinutes(1)
                }
            };
        }

        [Fact]
        public async Task Invoke_WithinLimit_ShouldAllowRequest()
        {
            // Arrange
            var context = CreateHttpContext("GET", "/api/test");
            var nextCalled = false;
            
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new RateLimitingMiddleware(next, _memoryCache, _loggerMock.Object, _options);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal(200, context.Response.StatusCode);
            Assert.True(context.Response.Headers.ContainsKey("X-RateLimit-Limit"));
            Assert.Equal("5", context.Response.Headers["X-RateLimit-Limit"].ToString());
            Assert.True(context.Response.Headers.ContainsKey("X-RateLimit-Remaining"));
            Assert.Equal("4", context.Response.Headers["X-RateLimit-Remaining"].ToString());
            Assert.True(context.Response.Headers.ContainsKey("X-RateLimit-Reset"));
        }

        [Fact]
        public async Task Invoke_ExceedingLimit_ShouldReturnTooManyRequests()
        {
            // Arrange
            var context = CreateHttpContext("POST", "/api/test");
            var nextCalled = false;
            
            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new RateLimitingMiddleware(next, _memoryCache, _loggerMock.Object, _options);

            // Act - Fazer requisições até exceder o limite
            for (int i = 0; i < 3; i++) // Primeiras 3 requisições devem passar
            {
                var tempContext = CreateHttpContext("POST", "/api/test");
                await middleware.Invoke(tempContext);
                Assert.Equal(200, tempContext.Response.StatusCode);
            }
            
            // Resetar a flag nextCalled antes da última requisição
            nextCalled = false;
            
            // A 4ª requisição deve falhar
            await middleware.Invoke(context);

            // Assert
            Assert.False(nextCalled); // A última requisição não deve chamar o próximo middleware
            Assert.Equal(429, context.Response.StatusCode);
        }

        [Fact]
        public async Task Invoke_DifferentEndpoints_ShouldHaveSeparateCounters()
        {
            // Arrange
            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
            var middleware = new RateLimitingMiddleware(next, _memoryCache, _loggerMock.Object, _options);

            // Act
            var context1 = CreateHttpContext("GET", "/api/endpoint1");
            var context2 = CreateHttpContext("GET", "/api/endpoint2");

            await middleware.Invoke(context1);
            await middleware.Invoke(context2);

            // Assert
            Assert.Equal(200, context1.Response.StatusCode);
            Assert.Equal(200, context2.Response.StatusCode);
            
            // Ambos devem ter remaining = 4 (5 - 1)
            Assert.Equal("4", context1.Response.Headers["X-RateLimit-Remaining"].ToString());
            Assert.Equal("4", context2.Response.Headers["X-RateLimit-Remaining"].ToString());
        }

        [Fact]
        public async Task Invoke_AuthEndpoint_ShouldUseAuthPolicy()
        {
            // Arrange
            var context = CreateHttpContext("POST", "/api/token");
            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
            var middleware = new RateLimitingMiddleware(next, _memoryCache, _loggerMock.Object, _options);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(200, context.Response.StatusCode);
            // Deve usar a política de auth (10 requisições por 5 minutos)
            Assert.Equal("10", context.Response.Headers["X-RateLimit-Limit"].ToString());
            Assert.Equal("9", context.Response.Headers["X-RateLimit-Remaining"].ToString());
        }

        [Fact]
        public async Task Invoke_WithForwardedForHeader_ShouldUseCorrectClientId()
        {
            // Arrange
            var context = CreateHttpContext("GET", "/api/test");
            context.Request.Headers.Add("X-Forwarded-For", "192.168.1.100, 10.0.0.1");
            
            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
            var middleware = new RateLimitingMiddleware(next, _memoryCache, _loggerMock.Object, _options);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal(200, context.Response.StatusCode);
            Assert.Equal("5", context.Response.Headers["X-RateLimit-Limit"].ToString());
            Assert.Equal("4", context.Response.Headers["X-RateLimit-Remaining"].ToString());
            
            // Verificar que o middleware está usando o primeiro IP do X-Forwarded-For
            // Criamos um segundo contexto com o mesmo IP para verificar que o contador é compartilhado
            var context2 = CreateHttpContext("GET", "/api/test");
            context2.Request.Headers.Add("X-Forwarded-For", "192.168.1.100, 10.0.0.2");
            await middleware.Invoke(context2);
            
            Assert.Equal("3", context2.Response.Headers["X-RateLimit-Remaining"].ToString());
        }

        [Fact]
        public async Task Invoke_RateLimitExceeded_ShouldLogWarning()
        {
            // Arrange
            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
            var middleware = new RateLimitingMiddleware(next, _memoryCache, _loggerMock.Object, _options);

            // Act - Exceder o limite
            for (int i = 0; i < 3; i++) // Primeiras 3 requisições devem passar
            {
                var tempContext = CreateHttpContext("POST", "/api/test");
                await middleware.Invoke(tempContext);
                Assert.Equal(200, tempContext.Response.StatusCode);
            }
            
            // A 4ª requisição deve falhar e gerar o log
            var finalContext = CreateHttpContext("POST", "/api/test");
            await middleware.Invoke(finalContext);
            Assert.Equal(429, finalContext.Response.StatusCode);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Rate limit excedido")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private static HttpContext CreateHttpContext(string method, string path)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Path = path;
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("localhost", 7000);
            context.Response.Body = new MemoryStream();
            context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
            
            return context;
        }
    }
}