using FCG.Presentation.Models.Responses;
using FGC.Application.UserManagement.DTOs;
using FGC.Application.UserManagement.UseCases;
using FGC.Presentation.Models.Requests;
using FGC.Presentation.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FGC.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        #region [Contructor]

        private readonly PromoteUserToAdminUseCase _promoteUserUseCase;
        private readonly DemoteAdminToUserUseCase _demoteAdminUseCase;
        private readonly CreateAdminUserUseCase _createAdminUseCase;
        private readonly DeactivateUserUseCase _deactivateUserUseCase;
        private readonly ReactivateUserUseCase _reactivateUserUseCase;

        public AdminController(PromoteUserToAdminUseCase promoteUserUseCase, DemoteAdminToUserUseCase demoteAdminUseCase, CreateAdminUserUseCase createAdminUseCase, DeactivateUserUseCase deactivateUserUseCase, ReactivateUserUseCase reactivateUserUseCase)
        {
            _promoteUserUseCase = promoteUserUseCase;
            _demoteAdminUseCase = demoteAdminUseCase;
            _createAdminUseCase = createAdminUseCase;
            _deactivateUserUseCase = deactivateUserUseCase;
            _reactivateUserUseCase = reactivateUserUseCase;
        }

        #endregion

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> CreateAdmin([FromBody] CreateAdminUserRequest request)
        {
            try
            {
                var validationError = ValidateCreateAdminRequest(request);
                if (validationError != null) return validationError;

                var currentAdminId = GetCurrentUserId();
                if (currentAdminId == null)
                    return StatusCode(401, ApiResponse<object>.ErrorMethod("Token inválido"));

                var dto = new CreateAdminUserDTO
                {
                    Email = request.Email,
                    Password = request.Password,
                    Name = request.Name,
                    CreatedByAdminId = currentAdminId.Value
                };

                var result = await _createAdminUseCase.ExecuteAsync(dto);
                var response = MapToUserResponse(result);

                return StatusCode(StatusCodes.Status201Created, ApiResponse<UserResponse>.SuccessMethod(response, $"Administrador '{result.Name}' criado com sucesso"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor {ex.Message}"));
            }
        }

        [HttpPut("promote")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> PromoteUser([FromBody] PromoteUserRequest request)
        {
            try
            {
                var validationError = ValidatePromoteUserRequest(request);
                if (validationError != null) return validationError;

                var currentAdminId = GetCurrentUserId();
                if (currentAdminId == null)
                    return StatusCode(401, ApiResponse<object>.ErrorMethod("Token inválido"));

                var dto = new PromoteUserToAdminDTO
                {
                    UserId = request.UserId,
                    AdminId = currentAdminId.Value
                };

                var result = await _promoteUserUseCase.ExecuteAsync(dto);
                var response = MapToUserResponse(result);

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, $"Usuário '{result.Name}' promovido a administrador"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                var statusCode = ex.Message.Contains("não encontrado") ? 404 : 409;
                return StatusCode(statusCode, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor {ex.Message}"));
            }
        }

        [HttpPut("demote/{adminId}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> DemoteAdminToUser(Guid adminId)
        {
            try
            {
                if (adminId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do administrador é obrigatório"));

                var currentAdminId = GetCurrentUserId();
                if (currentAdminId == null)
                    return StatusCode(401, ApiResponse<object>.ErrorMethod("Token inválido"));

                var dto = new DemoteAdminToUserDTO
                {
                    AdminId = adminId,
                    RequestingAdminId = currentAdminId.Value
                };

                var result = await _demoteAdminUseCase.ExecuteAsync(dto);
                var response = MapToUserResponse(result);

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, $"Administrador '{result.Name}' despromovido para usuário comum"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                var statusCode = ex.Message.Contains("não encontrado") ? 404 : 409;
                return StatusCode(statusCode, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor {ex.Message}"));
            }
        }

        [HttpPut("deactivate/{userId}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> DeactivateUser(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do usuário é obrigatório"));

                var result = await _deactivateUserUseCase.ExecuteAsync(new DeactivateUserDTO { UserId = userId });
                var response = MapToUserResponse(result);

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, $"Usuário '{result.Name}' desativado"));
            }
            catch (InvalidOperationException ex)
            {
                var statusCode = ex.Message.Contains("não encontrado") ? 404 : 409;
                return StatusCode(statusCode, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor {ex.Message}"));
            }
        }

        [HttpPut("reactivate/{userId}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> ReactivateUser(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do usuário é obrigatório"));

                var result = await _reactivateUserUseCase.ExecuteAsync(new ReactivateUserDTO { UserId = userId });
                var response = MapToUserResponse(result);

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, $"Usuário '{result.Name}' reativado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                var statusCode = ex.Message.Contains("não encontrado") ? 404 : 409;
                return StatusCode(statusCode, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor {ex.Message}"));
            }
        }

        [HttpGet("adminLogged")]
        public ActionResult<ApiResponse<object>> GetCurrentAdmin()
        {
            try
            {
                var currentUser = new
                {
                    UserId = GetCurrentUserId(),
                    Email = GetCurrentUserEmail(),
                    Name = GetCurrentUserName(),
                    Role = GetCurrentUserRole()
                };

                return Ok(ApiResponse<object>.SuccessMethod(currentUser, "Informações do administrador logado"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor {ex.Message}"));
            }
        }

        #region [Private Helpers - JWT]

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value;
        }

        private string GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        #endregion

        #region [Private Helpers - Validation]

        private ActionResult<ApiResponse<UserResponse>> ValidateCreateAdminRequest(CreateAdminUserRequest request)
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

        private ActionResult<ApiResponse<UserResponse>> ValidatePromoteUserRequest(PromoteUserRequest request)
        {
            if (request == null)
                return BadRequest(ApiResponse<object>.ErrorMethod("Dados obrigatórios"));

            if (request.UserId == Guid.Empty)
                return BadRequest(ApiResponse<object>.ErrorMethod("ID do usuário é obrigatório"));

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