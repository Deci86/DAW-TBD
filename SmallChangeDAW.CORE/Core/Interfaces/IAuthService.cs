using SmallChangeDAW.CORE.Core.DTOs;

namespace SmallChangeDAW.CORE.Core.Interfaces
{
    public interface IAuthService
    {
        // Método para registrar un nuevo usuario en la base de datos
        // Devuelve 'true' si el registro fue exitoso o 'false' si el email ya existe
        Task<bool> RegistrarUsuarioAsync(RegistroDTO registroDto);

        // Método para validar las credenciales de un usuario
        // Devuelve el DTO con el Token JWT si es correcto, o 'null' si las credenciales fallan
        Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto);
    }
}
