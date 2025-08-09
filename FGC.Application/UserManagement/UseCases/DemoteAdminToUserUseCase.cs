using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;

namespace FGC.Application.UserManagement.UseCases
{
    public class DemoteAdminToUserUseCase
    {
        #region [Constructor]

        private readonly IUserRepository _userRepository;

        public DemoteAdminToUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        #endregion

        public async Task<UserResponseDTO> ExecuteAsync(DemoteAdminToUserDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var requestingAdmin = await _userRepository.GetByIdAsync(dto.RequestingAdminId);
            if (requestingAdmin == null)
                throw new UnauthorizedAccessException("Administrador solicitante não encontrado");

            if (!requestingAdmin.IsAdmin())
                throw new UnauthorizedAccessException("Apenas administradores podem despromover outros usuários");

            if (!requestingAdmin.IsActive)
                throw new UnauthorizedAccessException("Administrador inativo não pode realizar esta operação");

            var adminTodemote = await _userRepository.GetByIdAsync(dto.AdminId);
            if (adminTodemote == null)
                throw new InvalidOperationException($"Usuário com ID {dto.AdminId} não encontrado");

            if (!adminTodemote.IsActive)
                throw new InvalidOperationException("Usuário inativo não pode ser despromovido");

            if (!adminTodemote.IsAdmin())
                throw new InvalidOperationException("Usuário não é administrador");

            if (dto.AdminId == dto.RequestingAdminId)
                throw new InvalidOperationException("Administrador não pode despromover a si mesmo");

            adminTodemote.DemoteToUser();

            await _userRepository.SaveAsync(adminTodemote);
            await ProcessDomainEventsAsync(adminTodemote, requestingAdmin);

            adminTodemote.ClearDomainEvents();

            return MapToResponseDto(adminTodemote);
        }

        private async Task ProcessDomainEventsAsync(User demotedUser, User requestingAdmin)
        {
            foreach (var domainEvent in demotedUser.DomainEvents)
            {
                Console.WriteLine($"[ADMIN_DEMOTED] Admin '{demotedUser.Email.Value}' demoted to User by admin '{requestingAdmin.Email.Value}' at {DateTime.UtcNow}");
                Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} - User: {demotedUser.Email.Value} demoted from Admin to User");
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