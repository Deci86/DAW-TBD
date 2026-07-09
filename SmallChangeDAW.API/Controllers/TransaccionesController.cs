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
    private readonly IAuditoriaService _auditoriaService;

    public TransaccionesController(ITransaccionesService transaccionesService, IAuditoriaService auditoriaService)
    {
        _transaccionesService = transaccionesService;
        _auditoriaService = auditoriaService;
    }

    [HttpGet]
    [Authorize] // Protegido para que solo usuarios logueados vean el historial
    public async Task<IActionResult> GetAll()
    {
        // El usuario solo puede obtener las transacciones donde participa
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // var transacciones = await _transaccionesService.GetAllAsync();
        var transacciones = await _transaccionesService.GetAllAsync(userId);
        return Ok(transacciones);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var transaccion = await _transaccionesService.GetByIdAsync(id);

        if (transaccion is null)
            return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });

        if (transaccion.ClienteCompradorId != userId && transaccion.Oferta?.cliente_id != userId)
        {
            return Forbid(); // 403 Forbidden si intenta husmear transacciones ajenas
        }

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

            var usuarioId = int.Parse(userIdClaim);

            // 2. Inyectamos forzosamente el ID del comprador desde el token
            // Asumiendo que tu DTO tiene una propiedad 'cliente_comprador_id'
            createDto.ClienteCompradorId = usuarioId;

            var transaccion = await _transaccionesService.AddAsync(createDto, usuarioId);

            // Ajusta 'id' o 'Id' según la convención de tu modelo
            return CreatedAtAction(nameof(GetById), new { id = transaccion.Id }, transaccion);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransaccionDTO updateDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var usuarioId = int.Parse(userIdClaim);
            var updated = await _transaccionesService.UpdateAsync(id, updateDto, usuarioId);
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();

        var usuarioId = int.Parse(userIdClaim);
        var deleted = await _transaccionesService.DeleteAsync(id, usuarioId);
        if (!deleted)
            return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });
        return NoContent();
    }

    /// <summary>
    /// Obtiene el historial de auditoría de una transacción específica
    /// </summary>
    [HttpGet("{id}/auditoria")]
    [Authorize]
    public async Task<IActionResult> GetHistorialAuditoria(int id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var usuarioId = int.Parse(userIdClaim);

            // Verificar que el usuario está involucrado en la transacción
            var transaccion = await _transaccionesService.GetByIdAsync(id);
            if (transaccion == null)
                return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });

            if (transaccion.ClienteCompradorId != usuarioId && transaccion.Oferta?.cliente_id != usuarioId)
            {
                return Forbid(); // No puede ver auditoría de transacciones ajenas
            }

            var historial = await _auditoriaService.ObtenerHistorialAsync(id);
            return Ok(historial);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene todas las auditorías realizadas por el usuario autenticado
    /// </summary>
    [HttpGet("auditoria/mi-historial")]
    [Authorize]
    public async Task<IActionResult> GetMiHistorialAuditoria()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var usuarioId = int.Parse(userIdClaim);
            var auditorias = await _auditoriaService.ObtenerAuditoriasPorUsuarioAsync(usuarioId);
            return Ok(auditorias);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}