using FGC.Domain.GameManagement.Entities;
using FGC.Domain.GameManagement.Enums;

namespace FGC.Domain.GameManagement.Interfaces
{
    public interface IGameRepository
    {
        Task<Game> GetByIdAsync(Guid id);

        Task<IEnumerable<Game>> GetAllAsync();

        Task<IEnumerable<Game>> GetByCategoryAsync(GameCategory category);

        Task<IEnumerable<Game>> GetActiveGamesAsync();

        Task<IEnumerable<Game>> SearchByTitleAsync(string searchTerm);

        Task<IEnumerable<Game>> GetTopRatedAsync(int count);

        Task<IEnumerable<Game>> GetMostSoldAsync(int count);

        Task<bool> ExistsByTitleAsync(string title);

        Task SaveAsync(Game game);

        Task DeleteAsync(Guid id);
    }
}
