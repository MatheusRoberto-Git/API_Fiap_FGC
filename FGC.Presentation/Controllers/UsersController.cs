using FCG.Presentation.Models.Responses;
using FGC.Application.UserManagement.DTOs;
using FGC.Application.UserManagement.UseCases;
using FGC.Presentation.Models.Requests;
using FGC.Presentation.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FGC.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        #region [Contructor]

        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly GetUserProfileUseCase _getUserProfileUseCase;
        private readonly ChangePasswordUseCase _changePasswordUseCase;
        private readonly DeactivateUserUseCase _deactivateUserUseCase;

        public UsersController(RegisterUserUseCase registerUserUseCase, GetUserProfileUseCase getUserProfileUseCase, ChangePasswordUseCase changePasswordUseCase, DeactivateUserUseCase deactivateUserUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _getUserProfileUseCase = getUserProfileUseCase;
            _changePasswordUseCase = changePasswordUseCase;
            _deactivateUserUseCase = deactivateUserUseCase;
        }

        #endregion

        [HttpGet("GetProfile/{id}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetProfile(Guid id)
        {
            try
            {
                var dto = new GetUserProfileDTO { UserId = id };
                var result = await _getUserProfileUseCase.ExecuteAsync(dto);

                var response = new UserResponse
                {
                    Id = result.Id,
                    Email = result.Email,
                    Name = result.Name,
                    Role = result.Role,
                    CreatedAt = result.CreatedAt,
                    IsActive = result.IsActive
                };

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<UserResponse>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserResponse>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPost("UserRegister")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                var dto = new RegisterUserDTO
                {
                    Email = request.Email,
                    Password = request.Password,
                    Name = request.Name
                };

                var result = await _registerUserUseCase.ExecuteAsync(dto);

                var response = new UserResponse
                {
                    Id = result.Id,
                    Email = result.Email,
                    Name = result.Name,
                    Role = result.Role,
                    CreatedAt = result.CreatedAt,
                    IsActive = result.IsActive
                };

                return CreatedAtAction(
                    nameof(GetProfile),
                    new { id = result.Id },
                    ApiResponse<UserResponse>.SuccessMethod(response, "Usuário cadastrado com sucesso")
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<UserResponse>.ErrorMethod(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<UserResponse>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserResponse>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }        

        [HttpPut("ChangePassword/{id}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                var dto = new ChangePasswordDTO
                {
                    UserId = id,
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword
                };

                var result = await _changePasswordUseCase.ExecuteAsync(dto);

                var response = new UserResponse
                {
                    Id = result.Id,
                    Email = result.Email,
                    Name = result.Name,
                    Role = result.Role,
                    CreatedAt = result.CreatedAt,
                    IsActive = result.IsActive
                };

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, "Senha alterada com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<UserResponse>.ErrorMethod(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ApiResponse<UserResponse>.ErrorMethod(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<UserResponse>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserResponse>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }        
    }
}
