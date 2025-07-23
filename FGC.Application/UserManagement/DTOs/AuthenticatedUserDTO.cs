namespace FGC.Application.UserManagement.DTOs
{
    public class AuthenticatedUserDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; }
    }
}
