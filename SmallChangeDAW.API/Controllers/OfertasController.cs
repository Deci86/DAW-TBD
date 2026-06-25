using Microsoft.AspNetCore.Authorization; // Necesario para [Authorize]
using Microsoft.AspNetCore.Mvc;
using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using System.Security.Claims; // Necesario para extraer los datos del Token

namespace SmallChangeDAW.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfertasController : ControllerBase
{
    private readonly IOfertasService _ofertasService;

    public OfertasController(IOfertasService ofertasService)
    {
        _ofertasService = ofertasService;
    }

    // GET: Público (cualquiera puede ver las ofertas disponibles)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ofertas = await _ofertasService.GetAllAsync();
        return Ok(ofertas);
    }

    // GET por ID: Público
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var oferta = await _ofertasService.GetByIdAsync(id);
        if (oferta is null)
            return NotFound(new { mensaje = $"Oferta con ID {id} no encontrada." });
        return Ok(oferta);
    }

    // POST: Requiere sesión iniciada
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateOfertaDTO createDto)
    {
        try
        {
            // Extraemos el ID del usuario directamente del token (es más seguro que confiar en el Frontend)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var oferta = await _ofertasService.AddAsync(createDto, userId);

            return CreatedAtAction(nameof(GetById), new { id = oferta.Id }, oferta);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT: Requiere sesión y ser el creador de la oferta
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOfertaDTO updateDto)
    {
        try
        {
            // 1. Obtener la oferta original para verificar a quién le pertenece
            var ofertaOriginal = await _ofertasService.GetByIdAsync(id);
            if (ofertaOriginal == null)
                return NotFound(new { mensaje = $"Oferta con ID {id} no encontrada." });

            // 2. Extraer el ID del usuario que está haciendo la petición
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 3. Validar que el usuario logueado sea el dueño de la oferta
            // IMPORTANTE: Cambia 'cliente_id' por 'ClienteId' si en tu DTO usaste PascalCase
            if (ofertaOriginal.ClienteId.ToString() != userIdClaim)
            {
                return Forbid(); // Retorna código 403: No tienes permisos para alterar algo que no es tuyo
            }

            var updated = await _ofertasService.UpdateAsync(id, updateDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // DELETE: Requiere sesión y ser el creador de la oferta
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        // 1. Obtener la oferta original
        var ofertaOriginal = await _ofertasService.GetByIdAsync(id);
        if (ofertaOriginal == null)
            return NotFound(new { mensaje = $"Oferta con ID {id} no encontrada." });

        // 2. Extraer el ID del token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // 3. Validar propiedad
        if (ofertaOriginal.ClienteId.ToString() != userIdClaim)
        {
            return Forbid();
        }

        var deleted = await _ofertasService.DeleteAsync(id);
        return NoContent();
    }
}