namespace FGC.Application.UserManagement.DTOs
{
    public class ChangePasswordDTO
    {
        public Guid UserId { get; set; }

        public string CurrentPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}
