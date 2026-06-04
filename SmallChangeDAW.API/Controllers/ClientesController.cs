using Microsoft.AspNetCore.Mvc;
using SmallChangeDAW.Core.DTOs;
using SmallChangeDAW.Core.Interfaces;

namespace SmallChangeDAW.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClientesService _clientesService;

    public ClientesController(IClientesService clientesService)
    {
        _clientesService = clientesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clientes = await _clientesService.GetAllAsync();
        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _clientesService.GetByIdAsync(id);
        if (cliente is null)
            return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado." });
        return Ok(cliente);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClienteDTO createDto)
    {
        var cliente = await _clientesService.AddAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteDTO updateDto)
    {
        var updated = await _clientesService.UpdateAsync(id, updateDto);
        if (!updated)
            return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado." });
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _clientesService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado." });
        return NoContent();
    }
}
