using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;
using FGC.Domain.UserManagement.ValueObjects;

namespace FGC.Application.UserManagement.UseCases
{
    public class RegisterUserUseCase
    {
        #region [Constructor]

        private readonly IUserRepository _userRepository;
        private readonly IUserUniquenessService _uniquenessService;

        public RegisterUserUseCase(IUserRepository userRepository, IUserUniquenessService uniquenessService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _uniquenessService = uniquenessService ?? throw new ArgumentNullException(nameof(uniquenessService));
        }
        
        #endregion

        public async Task<UserResponseDTO> ExecuteAsync(RegisterUserDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var email = new Email(dto.Email);
            var password = new Password(dto.Password);

            if (await _uniquenessService.IsEmailTakenAsync(email))
                throw new InvalidOperationException($"Email {email.Value} já está em uso");

            var user = User.Create(email, password, dto.Name);

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
