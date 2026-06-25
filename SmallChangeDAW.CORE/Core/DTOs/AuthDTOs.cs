using System.ComponentModel.DataAnnotations;

namespace SmallChangeDAW.CORE.Core.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegistroDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;

        // Puedes descomentar estas líneas si tu frontend en Quasar 
        // necesita saber de inmediato quién es el usuario que se acaba de loguear
        // public int ClienteId { get; set; }
        // public string Nombre { get; set; } = string.Empty;
    }
}