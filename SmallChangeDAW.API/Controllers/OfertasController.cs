using Microsoft.AspNetCore.Mvc;
using SmallChangeDAW.Core.DTOs;
using SmallChangeDAW.Core.Interfaces;

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

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var ofertas = await _ofertasService.GetAllAsync();
        return Ok(ofertas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var oferta = await _ofertasService.GetByIdAsync(id);
        if (oferta is null)
            return NotFound(new { mensaje = $"Oferta con ID {id} no encontrada." });
        return Ok(oferta);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOfertaDTO createDto)
    {
        try
        {
            var oferta = await _ofertasService.AddAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = oferta.Id }, oferta);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOfertaDTO updateDto)
    {
        try
        {
            var updated = await _ofertasService.UpdateAsync(id, updateDto);
            if (!updated)
                return NotFound(new { mensaje = $"Oferta con ID {id} no encontrada." });
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
        var deleted = await _ofertasService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { mensaje = $"Oferta con ID {id} no encontrada." });
        return NoContent();
    }
}
