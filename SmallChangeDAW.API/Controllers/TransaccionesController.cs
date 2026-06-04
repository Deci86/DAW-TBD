using Microsoft.AspNetCore.Mvc;
using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;

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
    public async Task<IActionResult> GetAll()
    {
        var transacciones = await _transaccionesService.GetAllAsync();
        return Ok(transacciones);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var transaccion = await _transaccionesService.GetByIdAsync(id);
        if (transaccion is null)
            return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });
        return Ok(transaccion);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransaccionDTO createDto)
    {
        try
        {
            var transaccion = await _transaccionesService.AddAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = transaccion.Id }, transaccion);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
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
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _transaccionesService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { mensaje = $"Transaccion con ID {id} no encontrada." });
        return NoContent();
    }
}
