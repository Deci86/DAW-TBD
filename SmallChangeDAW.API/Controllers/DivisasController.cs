using Microsoft.AspNetCore.Mvc;
using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;

namespace SmallChangeDAW.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DivisasController : ControllerBase
{
    private readonly IDivisasService _divisasService;

    public DivisasController(IDivisasService divisasService)
    {
        _divisasService = divisasService;
    }

    [HttpGet("tipo-cambio")]
    public async Task<IActionResult> ObtenerTipoCambio([FromQuery] string monedaOrigen, [FromQuery] string monedaDestino)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(monedaOrigen) || string.IsNullOrWhiteSpace(monedaDestino))
                return BadRequest(new { mensaje = "Las monedas de origen y destino son requeridas." });

            var tipoCambio = await _divisasService.ObtenerTipoCambioAsync(monedaOrigen, monedaDestino);
            return Ok(tipoCambio);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("convertir")]
    public async Task<IActionResult> ConvertirMoneda([FromBody] CambioMonedaRequestDTO request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.MonedaIn) || string.IsNullOrWhiteSpace(request.MonedaOut))
                return BadRequest(new { mensaje = "Las monedas de origen y destino son requeridas." });

            if (request.Monto <= 0)
                return BadRequest(new { mensaje = "El monto debe ser mayor a cero." });

            var resultado = await _divisasService.ConvertirMonedaAsync(request.MonedaIn, request.MonedaOut, request.Monto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
