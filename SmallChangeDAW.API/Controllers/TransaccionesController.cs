using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using System.Security.Claims;

namespace SmallChangeDAW.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionesController : ControllerBase
{
    private readonly ITransaccionesService _transaccionesService;

    public TransaccionesController(ITransaccionesService transaccionesService)
    {
        _transaccionesService = transaccionesService;
    }

    [HttpGet]
    [Authorize] // Protegido para que solo usuarios logueados vean el historial
    public async Task<IActionResult> GetAll()
    {
        var transacciones = await _transaccionesService.GetAllAsync();
        return Ok(transacciones);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var transaccion = await _transaccionesService.GetByIdAsync(id);
        if (transaccion is null)
            return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });
        return Ok(transaccion);
    }

    [HttpPost]
    [Authorize] // Solo usuarios con token pueden crear transacciones
    public async Task<IActionResult> Create([FromBody] CreateTransaccionDTO createDto)
    {
        try
        {
            // 1. Extraemos el ID del usuario del token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            // 2. Inyectamos forzosamente el ID del comprador desde el token
            // Asumiendo que tu DTO tiene una propiedad 'cliente_comprador_id'
            createDto.ClienteCompradorId = int.Parse(userIdClaim);

            var transaccion = await _transaccionesService.AddAsync(createDto);

            // Ajusta 'id' o 'Id' según la convención de tu modelo
            return CreatedAtAction(nameof(GetById), new { id = transaccion.Id }, transaccion);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransaccionDTO updateDto)
    {
        try
        {
            var updated = await _transaccionesService.UpdateAsync(id, updateDto);
            if (!updated)
                return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _transaccionesService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });
        return NoContent();
    }
}