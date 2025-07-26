using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Domain.UserManagement.ValueObjects;
using FGC.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FGC.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region [Constructor]

        private readonly FGCDbContext _context;

        public UserRepository(FGCDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region [Methods]

        public async Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(Email email)
        {
            if (email == null)
                return null;

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(Email email)
        {
            if (email == null)
                return false;

            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }

        public async Task SaveAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser == null)
            {
                await _context.Users.AddAsync(user);
            }
            else
            {
                _context.Users.Update(user);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (IsUniqueConstraintViolation(ex))
                {
                    throw new InvalidOperationException("Email já está em uso por outro usuário.", ex);
                }

                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                return;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                user.Deactivate();
                await SaveAsync(user);
            }
        }

        public async Task<User?> GetByIdForUpdateAsync(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException?.Message?.Contains("IX_Users_Email") == true || ex.InnerException?.Message?.Contains("UNIQUE") == true;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #endregion
    }
}
