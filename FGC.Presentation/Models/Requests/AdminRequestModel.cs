﻿namespace FGC.Presentation.Models.Requests
{
    public class PromoteUserRequest
    {
        public Guid UserId { get; set; }
    }

    public class CreateAdminUserRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}