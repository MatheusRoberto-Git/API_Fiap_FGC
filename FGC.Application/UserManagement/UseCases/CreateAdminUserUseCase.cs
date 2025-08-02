using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Domain.UserManagement.ValueObjects;

namespace FGC.Application.UserManagement.UseCases
{
    public class CreateAdminUserUseCase
    {
        #region [Constructor]

        private readonly IUserRepository _userRepository;
        private readonly IUserUniquenessService _uniquenessService;

        public CreateAdminUserUseCase(IUserRepository userRepository, IUserUniquenessService uniquenessService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _uniquenessService = uniquenessService ?? throw new ArgumentNullException(nameof(uniquenessService));
        }

        #endregion

        public async Task<UserResponseDTO> ExecuteAsync(CreateAdminUserDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var creatorAdmin = await _userRepository.GetByIdAsync(dto.CreatedByAdminId);

            if (creatorAdmin == null)
                throw new UnauthorizedAccessException("Administrador criador não encontrado");

            if (!creatorAdmin.IsAdmin())
                throw new UnauthorizedAccessException("Apenas administradores podem criar outros administradores");

            if (!creatorAdmin.IsActive)
                throw new UnauthorizedAccessException("Administrador inativo não pode criar outros usuários");

            var email = new Email(dto.Email);
            var password = new Password(dto.Password);

            if (await _uniquenessService.IsEmailTakenAsync(email))
                throw new InvalidOperationException($"Email {email.Value} já está em uso");

            var newAdmin = User.CreateAdmin(email, password, dto.Name, dto.CreatedByAdminId);

            await _userRepository.SaveAsync(newAdmin);
            await ProcessDomainEventsAsync(newAdmin, creatorAdmin);

            newAdmin.ClearDomainEvents();

            return MapToResponseDto(newAdmin);
        }

        private async Task ProcessDomainEventsAsync(User newAdmin, User creatorAdmin)
        {
            foreach (var domainEvent in newAdmin.DomainEvents)
            {
                Console.WriteLine($"[ADMIN_CREATED] New admin '{newAdmin.Email.Value}' created by admin '{creatorAdmin.Email.Value}' at {DateTime.UtcNow}");
                Console.WriteLine($"[EVENT] {domainEvent.GetType().Name} - Admin: {newAdmin.Email.Value}");
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
