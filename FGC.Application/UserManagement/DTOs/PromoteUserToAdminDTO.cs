namespace FGC.Application.UserManagement.DTOs
{
    public class PromoteUserToAdminDTO
    {
        public Guid UserId { get; set; }

        public Guid AdminId { get; set; }
    }
}
