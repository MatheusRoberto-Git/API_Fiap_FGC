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

        public UsersController(RegisterUserUseCase registerUserUseCase, GetUserProfileUseCase getUserProfileUseCase, ChangePasswordUseCase changePasswordUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _getUserProfileUseCase = getUserProfileUseCase;
            _changePasswordUseCase = changePasswordUseCase;
        }

        #endregion

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                var validationError = ValidateRegisterRequest(request);
                if (validationError != null) return validationError;

                var dto = new RegisterUserDTO
                {
                    Email = request.Email,
                    Password = request.Password,
                    Name = request.Name
                };

                var result = await _registerUserUseCase.ExecuteAsync(dto);
                var response = MapToUserResponse(result);

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

        [HttpGet("profile/{id}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetProfile(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

                var result = await _getUserProfileUseCase.ExecuteAsync(new GetUserProfileDTO { UserId = id });
                var response = MapToUserResponse(result);

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

        [HttpPut("changePassword/{id}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                var validationError = ValidateChangePasswordRequest(id, request);
                if (validationError != null) return validationError;

                var dto = new ChangePasswordDTO
                {
                    UserId = id,
                    CurrentPassword = request.CurrentPassword,
                    NewPassword = request.NewPassword
                };

                var result = await _changePasswordUseCase.ExecuteAsync(dto);
                var response = MapToUserResponse(result);

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

        #region [Private Helpers]

        private ActionResult<ApiResponse<UserResponse>> ValidateRegisterRequest(RegisterUserRequest request)
        {
            if (request == null)
                return BadRequest(ApiResponse<object>.ErrorMethod("Dados obrigatórios"));

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(ApiResponse<object>.ErrorMethod("Email é obrigatório"));

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(ApiResponse<object>.ErrorMethod("Senha é obrigatória"));

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(ApiResponse<object>.ErrorMethod("Nome é obrigatório"));

            return null;
        }

        private ActionResult<ApiResponse<UserResponse>> ValidateChangePasswordRequest(Guid id, ChangePasswordRequest request)
        {
            if (id == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

            if (request == null)
                return BadRequest(ApiResponse<object>.ErrorMethod("Dados obrigatórios"));

            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return BadRequest(ApiResponse<object>.ErrorMethod("Senha atual é obrigatória"));

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(ApiResponse<object>.ErrorMethod("Nova senha é obrigatória"));

            return null;
        }

        private static UserResponse MapToUserResponse(UserResponseDTO dto)
        {
            return new UserResponse
            {
                Id = dto.Id,
                Email = dto.Email,
                Name = dto.Name,
                Role = dto.Role,
                CreatedAt = dto.CreatedAt,
                IsActive = dto.IsActive
            };
        }

        #endregion
    }
}
