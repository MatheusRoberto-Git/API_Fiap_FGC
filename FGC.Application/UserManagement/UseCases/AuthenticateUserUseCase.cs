using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Domain.UserManagement.ValueObjects;

namespace FGC.Application.UserManagement.UseCases
{
    public class AuthenticateUserUseCase
    {
        #region [Constructor]

        private readonly IUserRepository _userRepository;

        public AuthenticateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        #endregion

        public async Task<AuthenticatedUserDTO> ExecuteAsync(LoginDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var email = new Email(dto.Email);
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new UnauthorizedAccessException("Email ou senha inválidos");

            if (!user.CanLogin())
                throw new UnauthorizedAccessException("Usuário inativo");

            if (user.Password.Value != dto.Password)
                throw new UnauthorizedAccessException("Email ou senha inválidos");

            user.RecordLogin();

            await _userRepository.SaveAsync(user);
            await ProcessDomainEventsAsync(user);

            user.ClearDomainEvents();

            return MapToAuthenticatedDto(user);
        }

        private async Task ProcessDomainEventsAsync(User user)
        {
            foreach (var domainEvent in user.DomainEvents)
            {
                Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} - User: {user.Email.Value}");
            }
        }

        private static AuthenticatedUserDTO MapToAuthenticatedDto(User user)
        {
            return new AuthenticatedUserDTO
            {
                Id = user.Id,
                Email = user.Email.Value,
                Name = user.Name,
                Role = user.Role.ToString(),
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive
            };
        }
    }
}
