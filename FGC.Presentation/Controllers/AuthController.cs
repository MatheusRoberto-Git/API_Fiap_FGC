using FCG.Presentation.Models.Responses;
using FGC.Application.UserManagement.DTOs;
using FGC.Application.UserManagement.UseCases;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Presentation.Models.Requests;
using FGC.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FGC.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        #region [Contructor]

        private readonly AuthenticateUserUseCase _authenticateUserUseCase;
        private readonly IJwtService _jwtService;

        public AuthController(
            AuthenticateUserUseCase authenticateUserUseCase, IJwtService jwtService)
        {
            _authenticateUserUseCase = authenticateUserUseCase;
            _jwtService = jwtService;
        }

        #endregion

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponse<object>.ErrorMethod("Dados de login são obrigatórios"));

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorMethod("Email é obrigatório"));

                if (string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(ApiResponse<object>.ErrorMethod("Senha é obrigatória"));

                var dto = new LoginDTO
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var authenticatedUser = await _authenticateUserUseCase.ExecuteAsync(dto);
                var userRepository = HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetByIdAsync(authenticatedUser.Id);

                if (user == null)
                    throw new InvalidOperationException("Usuário não encontrado após autenticação");

                var token = _jwtService.GenerateToken(user);
                var expiresAt = DateTime.UtcNow.AddMinutes(120);

                var response = new AuthResponse
                {
                    User = new UserResponse
                    {
                        Id = authenticatedUser.Id,
                        Email = authenticatedUser.Email,
                        Name = authenticatedUser.Name,
                        Role = authenticatedUser.Role,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = authenticatedUser.IsActive
                    },
                    Token = token,
                    ExpiresAt = expiresAt,
                    LastLoginAt = authenticatedUser.LastLoginAt ?? DateTime.UtcNow
                };

                return Ok(ApiResponse<AuthResponse>.SuccessMethod(response, "Login realizado com sucesso"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            return Ok(ApiResponse<object>.SuccessMethod(null, "Logout realizado com sucesso"));
        }

        [HttpPost("validateToken")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public ActionResult<ApiResponse<object>> ValidateToken([FromBody] string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return BadRequest(ApiResponse<object>.ErrorMethod("Token é obrigatório"));

                var principal = _jwtService.ValidateToken(token);

                if (principal == null)
                {
                    return StatusCode(
                        StatusCodes.Status401Unauthorized,
                        ApiResponse<object>.ErrorMethod("Token inválido ou expirado")
                    );
                }

                var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                return Ok(ApiResponse<object>.SuccessMethod(new { UserId = userId, Email = email, Role = role }, "Token válido"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ApiResponse<object>.ErrorMethod($"Token inválido - {ex.Message}"));
            }
        }
    }
}
