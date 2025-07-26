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
    public class AdminController : ControllerBase
    {
        #region [Contructor]

        private readonly PromoteUserToAdminUseCase _promoteUserToAdminUseCase;
        private readonly DeactivateUserUseCase _deactivateUserUseCase;
        private readonly GetUserProfileUseCase _getUserProfileUseCase;
        private readonly CreateAdminUserUseCase _createAdminUserUseCase;

        public AdminController(PromoteUserToAdminUseCase promoteUserToAdminUseCase, DeactivateUserUseCase deactivateUserUseCase, GetUserProfileUseCase getUserProfileUseCase, CreateAdminUserUseCase createAdminUserUseCase)
        {
            _promoteUserToAdminUseCase = promoteUserToAdminUseCase;
            _deactivateUserUseCase = deactivateUserUseCase;
            _getUserProfileUseCase = getUserProfileUseCase;
            _createAdminUserUseCase = createAdminUserUseCase;
        }

        #endregion

        [HttpPost("CreateAdmin")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> CreateAdmin([FromBody] CreateAdminUserRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponse<object>.ErrorMethod("Dados da requisição são obrigatórios"));

                if (string.IsNullOrWhiteSpace(request.Email))
                    return BadRequest(ApiResponse<object>.ErrorMethod("Email é obrigatório"));

                if (string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest(ApiResponse<object>.ErrorMethod("Senha é obrigatória"));

                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(ApiResponse<object>.ErrorMethod("Nome é obrigatório"));

                if (request.CreatedByAdminId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do administrador criador é obrigatório"));

                var dto = new CreateAdminUserDTO
                {
                    Email = request.Email,
                    Password = request.Password,
                    Name = request.Name,
                    CreatedByAdminId = request.CreatedByAdminId
                };

                var result = await _createAdminUserUseCase.ExecuteAsync(dto);

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
                    nameof(GetAdminProfile),
                    new { id = result.Id },
                    ApiResponse<UserResponse>.SuccessMethod(response, $"Administrador '{result.Name}' criado com sucesso")
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized,
                    ApiResponse<object>.ErrorMethod(ex.Message));
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
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPut("PromoteUser")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> PromoteUser([FromBody] PromoteUserRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponse<object>.ErrorMethod("Dados da requisição são obrigatórios"));

                if (request.UserId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do usuário é obrigatório"));

                if (request.AdminId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do administrador é obrigatório"));

                var dto = new PromoteUserToAdminDTO
                {
                    UserId = request.UserId,
                    AdminId = request.AdminId
                };

                var result = await _promoteUserToAdminUseCase.ExecuteAsync(dto);

                var response = new UserResponse
                {
                    Id = result.Id,
                    Email = result.Email,
                    Name = result.Name,
                    Role = result.Role,
                    CreatedAt = result.CreatedAt,
                    IsActive = result.IsActive
                };

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, $"Usuário '{result.Name}' promovido a administrador com sucesso"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                    return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));

                return Conflict(ApiResponse<object>.ErrorMethod(ex.Message));
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

        [HttpPut("DeactivateUser/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> DeactivateUser(Guid userId, [FromQuery] Guid adminId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do usuário é obrigatório"));

                if (adminId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do administrador é obrigatório"));

                var adminProfile = await _getUserProfileUseCase.ExecuteAsync(new GetUserProfileDTO { UserId = adminId });
                if (adminProfile.Role != "Admin")
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                        ApiResponse<object>.ErrorMethod("Apenas administradores podem desativar usuários"));
                }

                var dto = new DeactivateUserDTO { UserId = userId };
                var result = await _deactivateUserUseCase.ExecuteAsync(dto);

                var response = new UserResponse
                {
                    Id = result.Id,
                    Email = result.Email,
                    Name = result.Name,
                    Role = result.Role,
                    CreatedAt = result.CreatedAt,
                    IsActive = result.IsActive
                };

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, $"Usuário '{result.Name}' desativado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("não encontrado"))
                    return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));

                return Conflict(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("GetUsers")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>> ListUsers([FromQuery] Guid adminId, [FromQuery] bool? active = null)
        {
            try
            {
                if (adminId == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do administrador é obrigatório"));

                var adminProfile = await _getUserProfileUseCase.ExecuteAsync(new GetUserProfileDTO { UserId = adminId });

                if (adminProfile.Role != "Admin")
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                        ApiResponse<object>.ErrorMethod("Apenas administradores podem listar usuários"));
                }

                var message = active.HasValue ? $"Listagem de usuários {(active.Value ? "ativos" : "inativos")} - Em desenvolvimento" : "Listagem de todos os usuários - Em desenvolvimento";

                return Ok(ApiResponse<object>.SuccessMethod(new { Message = message, TotalUsers = 0, Users = new List<object>() }, "Funcionalidade em desenvolvimento"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("AdminProfile/{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetAdminProfile(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID do usuário é obrigatório"));

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

                return Ok(ApiResponse<UserResponse>.SuccessMethod(response, "Perfil obtido com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }
    }
}