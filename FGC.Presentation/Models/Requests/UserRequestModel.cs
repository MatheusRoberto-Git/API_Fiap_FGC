using System.ComponentModel.DataAnnotations;

namespace FGC.Presentation.Models.Requests
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(254, ErrorMessage = "Email muito longo")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(8, ErrorMessage = "Senha deve ter pelo menos 8 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome é obrigatório")]
        [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
        [MaxLength(100, ErrorMessage = "Nome muito longo")]
        public string Name { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Senha atual é obrigatória")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [MinLength(8, ErrorMessage = "Nova senha deve ter pelo menos 8 caracteres")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class PromoteUserRequest
    {
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "ID do administrador é obrigatório")]
        public Guid AdminId { get; set; }
    }

    public class CreateAdminUserRequest
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(254, ErrorMessage = "Email muito longo")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(8, ErrorMessage = "Senha deve ter pelo menos 8 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome é obrigatório")]
        [MinLength(2, ErrorMessage = "Nome deve ter pelo menos 2 caracteres")]
        [MaxLength(100, ErrorMessage = "Nome muito longo")]
        public string Name { get; set; } = string.Empty;

        public Guid CreatedByAdminId { get; set; }
    }
}