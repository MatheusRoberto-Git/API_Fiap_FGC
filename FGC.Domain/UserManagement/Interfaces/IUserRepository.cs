using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.ValueObjects;

namespace FGC.Domain.UserManagement.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);

        Task<User> GetByEmailAsync(Email email);

        Task<bool> ExistsByEmailAsync(Email email);

        Task SaveAsync(User user);

        Task DeleteAsync(Guid id);

        Task<User> GetByIdForUpdateAsync(Guid id);
    }
}
