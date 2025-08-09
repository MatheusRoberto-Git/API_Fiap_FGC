namespace FGC.Application.UserManagement.DTOs
{
    public class DemoteAdminToUserDTO
    {
        public Guid AdminId { get; set; }

        public Guid RequestingAdminId { get; set; }
    }
}
