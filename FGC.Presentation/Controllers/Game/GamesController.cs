using FGC.Application.GameManagement.DTOs;
using FGC.Application.GameManagement.UseCases;
using FGC.Presentation.Models.Requests.Game;
using FGC.Presentation.Models.Responses.Game;
using FGC.Presentation.Models.Responses.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGC.Presentation.Controllers.Game
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        #region [Contructor]

        private readonly CreateGameUseCase _createGameUseCase;
        private readonly GetGameByIdUseCase _getGameByIdUseCase;
        private readonly GetAllGamesUseCase _getAllGamesUseCase;
        private readonly SearchGamesUseCase _searchGamesUseCase;
        private readonly UpdateGamePriceUseCase _updateGamePriceUseCase;

        public GamesController(CreateGameUseCase createGameUseCase, GetGameByIdUseCase getGameByIdUseCase, GetAllGamesUseCase getAllGamesUseCase, SearchGamesUseCase searchGamesUseCase, UpdateGamePriceUseCase updateGamePriceUseCase)
        {
            _createGameUseCase = createGameUseCase;
            _getGameByIdUseCase = getGameByIdUseCase;
            _getAllGamesUseCase = getAllGamesUseCase;
            _searchGamesUseCase = searchGamesUseCase;
            _updateGamePriceUseCase = updateGamePriceUseCase;
        }

        #endregion

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GameResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<GameResponse>>>> GetAll()
        {
            try
            {
                var games = await _getAllGamesUseCase.ExecuteAsync();
                var response = games.Select(MapToResponse);

                return Ok(ApiResponse<IEnumerable<GameResponse>>.SuccessMethod(response, "Jogos listados com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

                var game = await _getGameByIdUseCase.ExecuteAsync(id);
                var response = MapToResponse(game);

                return Ok(ApiResponse<GameResponse>.SuccessMethod(response));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GameResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<GameResponse>>>> Search([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return BadRequest(ApiResponse<object>.ErrorMethod("Termo de busca é obrigatório"));

                var games = await _searchGamesUseCase.ExecuteAsync(term);
                var response = games.Select(MapToResponse);

                return Ok(ApiResponse<IEnumerable<GameResponse>>.SuccessMethod(response, $"Encontrados {response.Count()} jogos"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> Create([FromBody] CreateGameRequest request)
        {
            try
            {
                var validationError = ValidateCreateRequest(request);
                if (validationError != null) return validationError;

                var dto = new CreateGameDTO
                {
                    Title = request.Title,
                    Description = request.Description,
                    Price = request.Price,
                    Category = request.Category,
                    Developer = request.Developer,
                    Publisher = request.Publisher,
                    ReleaseDate = request.ReleaseDate
                };

                var result = await _createGameUseCase.ExecuteAsync(dto);
                var response = MapToResponse(result);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    ApiResponse<GameResponse>.SuccessMethod(response, "Jogo criado com sucesso")
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        [HttpPut("{id}/price")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<GameResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<GameResponse>>> UpdatePrice(Guid id, [FromBody] UpdateGamePriceRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(ApiResponse<object>.ErrorMethod("ID inválido"));

                if (request.NewPrice < 0)
                    return BadRequest(ApiResponse<object>.ErrorMethod("Preço não pode ser negativo"));

                var dto = new UpdateGamePriceDTO
                {
                    GameId = id,
                    NewPrice = request.NewPrice
                };

                var result = await _updateGamePriceUseCase.ExecuteAsync(dto);
                var response = MapToResponse(result);

                return Ok(ApiResponse<GameResponse>.SuccessMethod(response, "Preço atualizado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorMethod(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorMethod($"Erro interno do servidor - {ex.Message}"));
            }
        }

        #region [Private Helpers]

        private ActionResult<ApiResponse<GameResponse>> ValidateCreateRequest(CreateGameRequest request)
        {
            if (request == null)
                return BadRequest(ApiResponse<object>.ErrorMethod("Dados obrigatórios"));

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest(ApiResponse<object>.ErrorMethod("Título é obrigatório"));

            if (string.IsNullOrWhiteSpace(request.Description))
                return BadRequest(ApiResponse<object>.ErrorMethod("Descrição é obrigatória"));

            if (request.Price < 0)
                return BadRequest(ApiResponse<object>.ErrorMethod("Preço não pode ser negativo"));

            if (string.IsNullOrWhiteSpace(request.Developer))
                return BadRequest(ApiResponse<object>.ErrorMethod("Desenvolvedor é obrigatório"));

            if (string.IsNullOrWhiteSpace(request.Publisher))
                return BadRequest(ApiResponse<object>.ErrorMethod("Publicador é obrigatório"));

            return null;
        }

        private static GameResponse MapToResponse(GameResponseDTO dto)
        {
            return new GameResponse
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Category = dto.Category,
                Developer = dto.Developer,
                Publisher = dto.Publisher,
                ReleaseDate = dto.ReleaseDate,
                CreatedAt = dto.CreatedAt,
                IsActive = dto.IsActive,
                Rating = dto.Rating,
                TotalSales = dto.TotalSales
            };
        }

        #endregion
    }
}
