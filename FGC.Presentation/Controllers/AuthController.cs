using FCG.Presentation.Models.Responses;
using FGC.Application.UserManagement.DTOs;
using FGC.Application.UserManagement.UseCases;
using FGC.Presentation.Models.Requests;
using FGC.Presentation.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FGC.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticateUserUseCase _authenticateUserUseCase;

        public AuthController(AuthenticateUserUseCase authenticateUserUseCase)
        {
            _authenticateUserUseCase = authenticateUserUseCase;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var dto = new LoginDTO
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _authenticateUserUseCase.ExecuteAsync(dto);

                var response = new AuthResponse
                {
                    User = new UserResponse
                    {
                        Id = result.Id,
                        Email = result.Email,
                        Name = result.Name,
                        Role = result.Role,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = result.IsActive
                    },
                    LastLoginAt = (DateTime)result.LastLoginAt,
                    Token = null
                };

                return Ok(ApiResponse<AuthResponse>.SuccessMethod(response, "Login realizado com sucesso"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ApiResponse<UserResponse>.ErrorMethod(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<AuthResponse>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponse>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            return Ok(ApiResponse<object>.SuccessMethod(null, "Logout realizado com sucesso"));
        }
    }
}
