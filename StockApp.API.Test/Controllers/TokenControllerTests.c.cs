using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using Xunit;

namespace StockApp.Tests.Controllers
{
    public class TokenControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService = new();
        private readonly Mock<IMfaService> _mockMfaService = new();
        private readonly TokenController _controller;

        public TokenControllerTests()
        {
            _controller = new TokenController(_mockAuthService.Object, _mockMfaService.Object);
        }

        [Fact]
        public async Task Login_ValidUser_ReturnsToken()
        {
            // Arrange
            var user = new UserLoginDTO { Username = "test", Password = "test" };
            var expectedToken = new TokenResponseDto { Token = "123" };
            
            _mockAuthService.Setup(x => x.AuthenticateAsync(user.Username, user.Password))
                           .ReturnsAsync(expectedToken);

            // Act
            var result = await _controller.Login(user);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_InvalidUser_ReturnsUnauthorized()
        {
            // Arrange
            var user = new UserLoginDTO { Username = "wrong", Password = "wrong" };
            
            _mockAuthService.Setup(x => x.AuthenticateAsync(user.Username, user.Password))
                           .ReturnsAsync((TokenResponseDto)null);

            // Act
            var result = await _controller.Login(user);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void GenerateOtp_ReturnsOtp()
        {
            // Arrange
            _mockMfaService.Setup(x => x.GenerateOtp()).Returns("123456");

            // Act
            var result = _controller.GenerateOtp();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ValidateOtp_CorrectCode_ReturnsOk()
        {
            // Arrange
            var request = new TokenController.OtpRequest { UserOtp = "123456", StoredOtp = "123456" };
            _mockMfaService.Setup(x => x.ValidateOtp(request.UserOtp, request.StoredOtp)).Returns(true);

            // Act
            var result = _controller.ValidateOtp(request);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void ValidateOtp_WrongCode_ReturnsBadRequest()
        {
            // Arrange
            var request = new TokenController.OtpRequest { UserOtp = "123", StoredOtp = "456" };
            _mockMfaService.Setup(x => x.ValidateOtp(request.UserOtp, request.StoredOtp)).Returns(false);

            // Act
            var result = _controller.ValidateOtp(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}