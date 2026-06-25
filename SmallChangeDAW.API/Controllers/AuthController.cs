using Microsoft.AspNetCore.Mvc;
using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;

namespace SmallChangeDAW.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        // Inyectamos la interfaz del servicio de autenticación
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDTO registroDto)
        {
            // Validamos que los datos cumplan con las reglas del DTO (Email válido, longitud de contraseña, etc.)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _authService.RegistrarUsuarioAsync(registroDto);

            if (!resultado)
            {
                // Si el servicio devuelve false, significa que el correo ya existe
                return BadRequest(new { mensaje = "El correo electrónico ya se encuentra registrado." });
            }

            return Ok(new { mensaje = "Usuario registrado exitosamente." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var respuesta = await _authService.LoginAsync(loginDto);

            if (respuesta == null)
            {
                // Si el servicio devuelve null, las credenciales son incorrectas
                // Devolvemos un error 401 Unauthorized
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos." });
            }

            // Si todo está bien, devolvemos un código 200 OK con el Token JWT adentro
            return Ok(respuesta);
        }
    }
}