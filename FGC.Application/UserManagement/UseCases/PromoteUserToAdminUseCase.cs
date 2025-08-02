using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;

namespace FGC.Application.UserManagement.UseCases
{
    public class PromoteUserToAdminUseCase
    {
        #region [Constructor]

        private readonly IUserRepository _userRepository;

        public PromoteUserToAdminUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        #endregion

        public async Task<UserResponseDTO> ExecuteAsync(PromoteUserToAdminDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var admin = await _userRepository.GetByIdAsync(dto.AdminId);
            if (admin == null)
                throw new UnauthorizedAccessException("Administrador não encontrado");

            if (!admin.IsAdmin())
                throw new UnauthorizedAccessException("Apenas administradores podem promover outros usuários");

            if (!admin.IsActive)
                throw new UnauthorizedAccessException("Administrador inativo não pode realizar esta operação");

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
                throw new InvalidOperationException($"Usuário com ID {dto.UserId} não encontrado");

            if (!user.IsActive)
                throw new InvalidOperationException("Usuário inativo não pode ser promovido");

            user.PromoteToAdmin();

            await _userRepository.SaveAsync(user);
            await ProcessDomainEventsAsync(user);

            user.ClearDomainEvents();

            return MapToResponseDto(user);
        }

        private async Task ProcessDomainEventsAsync(User user)
        {
            foreach (var domainEvent in user.DomainEvents)
            {
                Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} - User: {user.Email.Value} promoted to Admin");
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
