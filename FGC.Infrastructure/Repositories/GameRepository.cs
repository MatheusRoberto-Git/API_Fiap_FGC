using FGC.Domain.GameManagement.Entities;
using FGC.Domain.GameManagement.Enums;
using FGC.Domain.GameManagement.Interfaces;
using FGC.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FGC.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        #region [Constructor]

        private readonly FGCDbContext _context;

        public GameRepository(FGCDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region [Methods]

        public async Task<Game?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _context.Games
                .AsNoTracking()
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetByCategoryAsync(GameCategory category)
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.Category == category && g.IsActive)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetActiveGamesAsync()
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.IsActive)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> SearchByTitleAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Game>();

            return await _context.Games
                .AsNoTracking()
                .Where(g => g.Title.Contains(searchTerm) && g.IsActive)
                .OrderBy(g => g.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetTopRatedAsync(int count)
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.IsActive)
                .OrderByDescending(g => g.Rating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Game>> GetMostSoldAsync(int count)
        {
            return await _context.Games
                .AsNoTracking()
                .Where(g => g.IsActive)
                .OrderByDescending(g => g.TotalSales)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> ExistsByTitleAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            return await _context.Games
                .AsNoTracking()
                .AnyAsync(g => g.Title.ToLower() == title.ToLower());
        }

        public async Task SaveAsync(Game game)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));

            var existingGame = await _context.Games
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == game.Id);

            if (existingGame == null)
            {
                await _context.Games.AddAsync(game);
            }
            else
            {
                _context.Games.Update(game);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                return;

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);

            if (game != null)
            {
                game.Deactivate();
                await SaveAsync(game);
            }
        }

        #endregion
    }
}
