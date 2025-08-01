﻿namespace FGC.Application.UserManagement.DTOs
{
    public class CreateAdminUserDTO
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public Guid CreatedByAdminId { get; set; }
    }
}
