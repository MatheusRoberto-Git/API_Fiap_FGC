using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;

namespace FGC.Application.UserManagement.UseCases
{
    public class DeactivateUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public DeactivateUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserResponseDTO> ExecuteAsync(DeactivateUserDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (user == null)
                throw new InvalidOperationException($"Usuário com ID {dto.UserId} não encontrado");

            if (!user.IsActive)
                throw new InvalidOperationException("Usuário já está inativo");

            user.Deactivate();

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
