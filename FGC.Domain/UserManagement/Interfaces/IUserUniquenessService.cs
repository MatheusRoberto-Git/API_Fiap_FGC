using FGC.Domain.UserManagement.ValueObjects;

namespace FGC.Domain.UserManagement.Interfaces
{
    public interface IUserUniquenessService
    {
        Task<bool> IsEmailTakenAsync(Email email);
    }
}
