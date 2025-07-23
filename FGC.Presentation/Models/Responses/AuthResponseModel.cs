namespace FGC.Presentation.Models.Responses
{
    public class AuthResponse
    {
        public UserResponse User { get; set; } = new();

        public DateTime LastLoginAt { get; set; }

        public string Token { get; set; }

        public AuthResponse()
        {
            User = new UserResponse();
            Token = string.Empty;
        }
    }
}
