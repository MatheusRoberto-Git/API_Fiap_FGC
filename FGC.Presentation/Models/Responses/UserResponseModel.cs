namespace FGC.Presentation.Models.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public UserResponse()
        {
            Email = string.Empty;
            Name = string.Empty;
            Role = string.Empty;
        }
    }    
}