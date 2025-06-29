using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Security.Claims;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SocialAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILogger<SocialAuthController> _logger;

        public SocialAuthController(
            IAuthService authService,
            IUserService userService,
            ILogger<SocialAuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Inicia o processo de autenticação com Google
        /// </summary>
        /// <param name="returnUrl">URL de retorno após autenticação</param>
        /// <returns>Challenge result para Google</returns>
        [HttpGet("google")]
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "SocialAuth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Callback do Google após autenticação
        /// </summary>
        /// <param name="returnUrl">URL de retorno</param>
        /// <returns>Token JWT ou erro</returns>
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Falha na autenticação com Google" });
                }

                var claims = result.Principal.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email não fornecido pelo Google" });
                }

                var token = await ProcessSocialLogin(email, name, "Google", googleId);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no callback do Google");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Inicia o processo de autenticação com Facebook
        /// </summary>
        /// <param name="returnUrl">URL de retorno após autenticação</param>
        /// <returns>Challenge result para Facebook</returns>
        [HttpGet("facebook")]
        public IActionResult FacebookLogin(string returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(FacebookCallback), "SocialAuth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Callback do Facebook após autenticação
        /// </summary>
        /// <param name="returnUrl">URL de retorno</param>
        /// <returns>Token JWT ou erro</returns>
        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookCallback(string returnUrl = "/")
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Falha na autenticação com Facebook" });
                }

                var claims = result.Principal.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var facebookId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email não fornecido pelo Facebook" });
                }

                var token = await ProcessSocialLogin(email, name, "Facebook", facebookId);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no callback do Facebook");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Inicia o processo de autenticação com Microsoft
        /// </summary>
        /// <param name="returnUrl">URL de retorno após autenticação</param>
        /// <returns>Challenge result para Microsoft</returns>
        [HttpGet("microsoft")]
        public IActionResult MicrosoftLogin(string returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(MicrosoftCallback), "SocialAuth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, MicrosoftAccountDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Callback da Microsoft após autenticação
        /// </summary>
        /// <param name="returnUrl">URL de retorno</param>
        /// <returns>Token JWT ou erro</returns>
        [HttpGet("microsoft-callback")]
        public async Task<IActionResult> MicrosoftCallback(string returnUrl = "/")
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Falha na autenticação com Microsoft" });
                }

                var claims = result.Principal.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var microsoftId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email não fornecido pela Microsoft" });
                }

                var token = await ProcessSocialLogin(email, name, "Microsoft", microsoftId);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no callback da Microsoft");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Processa o login social comum para todos os provedores
        /// </summary>
        /// <param name="email">Email do usuário</param>
        /// <param name="name">Nome do usuário</param>
        /// <param name="provider">Provedor de autenticação</param>
        /// <param name="providerId">ID do provedor</param>
        /// <returns>Token de resposta</returns>
        private async Task<TokenResponseDto> ProcessSocialLogin(string email, string name, string provider, string providerId)
        {
            try
            {
                // Verificar se o usuário já existe
                var existingUser = await _userService.GetUserByEmailAsync(email);
                
                if (existingUser == null)
                {
                    // Criar novo usuário
                    var newUser = new UserRegisterDto
                    {
                        Username = email,
                        Email = email,
                        Password = Guid.NewGuid().ToString(), // Senha temporária
                        Role = "User"
                    };
                    
                    await _userService.CreateUserAsync(newUser);
                    _logger.LogInformation($"Novo usuário criado via {provider}: {email}");
                }

                // Gerar token JWT
                var token = await _authService.AuthenticateAsync(email, null, provider);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar login social para {email} via {provider}");
                throw;
            }
        }
    }
}