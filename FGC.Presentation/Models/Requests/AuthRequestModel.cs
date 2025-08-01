﻿namespace FGC.Presentation.Models.Requests
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public class LogoutRequest
    {
        public string Token { get; set; }
    }
}