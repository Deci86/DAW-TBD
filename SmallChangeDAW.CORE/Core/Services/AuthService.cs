using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IClientesRepository _clientesRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IClientesRepository clientesRepository, IConfiguration configuration)
        {
            _clientesRepository = clientesRepository;
            _configuration = configuration;
        }

        public async Task<bool> RegistrarUsuarioAsync(RegistroDTO registroDto)  
        {
            // 1. Validar si el email ya se encuentra registrado
            var usuarioExistente = await _clientesRepository.GetByEmailAsync(registroDto.Email);
            if (usuarioExistente != null)
            {
                return false; // El correo ya está en uso
            }

            // 2. Generar el Hash seguro de la contraseña usando BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registroDto.Password);

            // 3. Mapear el DTO al modelo Cliente usando tus propiedades en minúscula/snake_case
            var nuevoCliente = new Cliente
            {
                nombre = registroDto.Nombre,
                email = registroDto.Email,
                pass_hash = passwordHash,
                promedio_calificacion_comprador = 0.00m,
                calificacion_vendedor = 0.00m,
                fecha_registro = DateTime.UtcNow
            };

            // 4. Persistir el nuevo cliente
            await _clientesRepository.AddAsync(nuevoCliente);
            return true;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO loginDto)
        {
            // 1. Buscar al cliente por su correo electrónico
            var cliente = await _clientesRepository.GetByEmailAsync(loginDto.Email);
            if (cliente == null)
            {
                return null; // El usuario no existe
            }

            // 2. Verificar si la contraseña ingresada coincide con el hash almacenado
            bool esPasswordValido = BCrypt.Net.BCrypt.Verify(loginDto.Password, cliente.pass_hash);
            if (!esPasswordValido)
            {
                return null; // Contraseña incorrecta
            }

            // 3. Credenciales correctas: Proceder a generar el Bearer Token
            var tokenGenerado = GenerarJwtToken(cliente);

            return new AuthResponseDTO
            {
                Token = tokenGenerado
            };
        }

        // Método privado encargado de la confección del JWT
        private string GenerarJwtToken(Cliente cliente)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var secretKey = _configuration["JwtSettings:SecretKey"]
                            ?? throw new InvalidOperationException("La clave secreta de JWT no está configurada.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Definir los Claims usando las propiedades exactas de tu modelo
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, cliente.id.ToString()),
                new Claim(ClaimTypes.Email, cliente.email),
                new Claim(ClaimTypes.Name, cliente.nombre)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}