using FGC.Application.GameManagement.DTOs;
using FGC.Domain.GameManagement.Entities;
using FGC.Domain.GameManagement.Enums;
using FGC.Domain.GameManagement.Interfaces;

namespace FGC.Application.GameManagement.UseCases
{
    public class CreateGameUseCase
    {
        private readonly IGameRepository _gameRepository;

        public CreateGameUseCase(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async Task<GameResponseDTO> ExecuteAsync(CreateGameDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (await _gameRepository.ExistsByTitleAsync(dto.Title))
                throw new InvalidOperationException($"Jogo com título '{dto.Title}' já existe");

            var category = (GameCategory)dto.Category;

            var game = Game.Create(
                dto.Title,
                dto.Description,
                dto.Price,
                category,
                dto.Developer,
                dto.Publisher,
                dto.ReleaseDate
            );

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
