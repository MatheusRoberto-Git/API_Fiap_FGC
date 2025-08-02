using FGC.Application.UserManagement.DTOs;
using FGC.Domain.UserManagement.Entities;
using FGC.Domain.UserManagement.Interfaces;

namespace FGC.Application.UserManagement.UseCases
{
    public class GetUserProfileUseCase
    {
        #region [Constructor]

        private readonly IUserRepository _userRepository;

        public GetUserProfileUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        #endregion

        public async Task<UserResponseDTO> ExecuteAsync(GetUserProfileDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var user = await _userRepository.GetByIdAsync(dto.UserId);

            if (user == null)
                throw new InvalidOperationException($"Usuário com ID {dto.UserId} não encontrado");

            if (!user.IsActive)
                throw new InvalidOperationException("Usuário inativo");

            return MapToResponseDto(user);
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
