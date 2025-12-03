using FGC.Application.GameManagement.DTOs;
using FGC.Domain.GameManagement.Entities;
using FGC.Domain.GameManagement.Interfaces;

namespace FGC.Application.GameManagement.UseCases
{
    public class GetGameByIdUseCase
    {
        private readonly IGameRepository _gameRepository;

        public GetGameByIdUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(Guid gameId)
        {
            if (gameId == Guid.Empty)
                throw new ArgumentException("GameId é obrigatório");

            var game = await _gameRepository.GetByIdAsync(gameId);

            if (game == null)
                throw new InvalidOperationException($"Jogo com ID {gameId} não encontrado");

            return MapToResponseDto(game);
        }

        private static GameResponseDTO MapToResponseDto(Game game)
        {
            return new GameResponseDTO
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Category = game.Category.ToString(),
                Developer = game.Developer,
                Publisher = game.Publisher,
                ReleaseDate = game.ReleaseDate,
                CreatedAt = game.CreatedAt,
                IsActive = game.IsActive,
                Rating = game.Rating,
                TotalSales = game.TotalSales
            };
        }
    }
}
