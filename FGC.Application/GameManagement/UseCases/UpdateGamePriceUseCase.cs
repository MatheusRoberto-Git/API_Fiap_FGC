using FGC.Application.GameManagement.DTOs;
using FGC.Domain.GameManagement.Entities;
using FGC.Domain.GameManagement.Interfaces;

namespace FGC.Application.GameManagement.UseCases
{
    public class UpdateGamePriceUseCase
    {
        private readonly IGameRepository _gameRepository;

        public UpdateGamePriceUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(UpdateGamePriceDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var game = await _gameRepository.GetByIdAsync(dto.GameId);

            if (game == null)
                throw new InvalidOperationException($"Jogo com ID {dto.GameId} não encontrado");

            game.UpdatePrice(dto.NewPrice);

            await _gameRepository.SaveAsync(game);
            await ProcessDomainEventsAsync(game);
            game.ClearDomainEvents();

            return MapToResponseDto(game);
        }

        private async Task ProcessDomainEventsAsync(Game game)
        {
            foreach (var domainEvent in game.DomainEvents)
            {
                Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} - Game: {game.Title}");
            }
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
