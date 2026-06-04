using SmallChangeDAW.CORE.Core.DTOs;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface IDivisasService
{
    Task<TipoCambioResponseDTO> ObtenerTipoCambioAsync(string monedaIn, string monedaOut);
    Task<CambioMonedaResponseDTO> ConvertirMonedaAsync(string monedaIn, string monedaOut, decimal monto);
}
