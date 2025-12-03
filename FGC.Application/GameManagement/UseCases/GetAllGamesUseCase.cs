using FGC.Application.GameManagement.DTOs;
using FGC.Domain.GameManagement.Entities;
using FGC.Domain.GameManagement.Interfaces;

namespace FGC.Application.GameManagement.UseCases
{
    public class GetAllGamesUseCase
    {
        private readonly IGameRepository _gameRepository;

        public GetAllGamesUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<IEnumerable<GameResponseDTO>> ExecuteAsync()
        {
            var games = await _gameRepository.GetActiveGamesAsync();
            return games.Select(MapToResponseDto);
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
