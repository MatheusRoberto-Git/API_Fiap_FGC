using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Domain.UserManagement.ValueObjects;

namespace FGC.Application.UserManagement.UseCases
{
    public class ChangePasswordUseCase
    {
        #region [Constructor]

        private readonly IUserRepository _userRepository;

        public ChangePasswordUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        #endregion

        public async Task<UserResponseDTO> ExecuteAsync(ChangePasswordDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (user == null)
                throw new InvalidOperationException($"Usuário com ID {dto.UserId} não encontrado");

            if (!user.IsActive)
                throw new InvalidOperationException("Usuário inativo não pode alterar senha");

            if (user.Password.Value != dto.CurrentPassword)
                throw new UnauthorizedAccessException("Senha atual incorreta");

            var newPassword = new Password(dto.NewPassword);

            user.ChangePassword(newPassword);

            await _userRepository.SaveAsync(user);
            await ProcessDomainEventsAsync(user);

            user.ClearDomainEvents();

            return MapToResponseDto(user);
        }

        private async Task ProcessDomainEventsAsync(User user)
        {
            foreach (var domainEvent in user.DomainEvents)
            {
                Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} - User: {user.Email.Value}");
            }
        }

        private static UserResponseDTO MapToResponseDto(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                Email = user.Email.Value,
                Name = user.Name,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive
            };
        }
    }
}
