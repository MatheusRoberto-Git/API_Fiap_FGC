using FGC.Domain.UserManagement.Interfaces;
using FGC.Domain.UserManagement.ValueObjects;

namespace FGC.Infrastructure.Services
{
    public class UserUniquenessService : IUserUniquenessService
    {
        private readonly IUserRepository _userRepository;

        public UserUniquenessService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<bool> IsEmailTakenAsync(Email email)
        {
            if (email == null)
                return false;

            return await _userRepository.ExistsByEmailAsync(email);
        }
    }
}
