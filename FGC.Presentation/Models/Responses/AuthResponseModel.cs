namespace FGC.Presentation.Models.Responses
{
    public class AuthResponse
    {
        public UserResponse User { get; set; } = new();

        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime LastLoginAt { get; set; }

        public string TokenType { get; set; } = "Bearer";

        public AuthResponse()
        {
            User = new UserResponse();
            Token = string.Empty;
            TokenType = "Bearer";
        }
    }
}