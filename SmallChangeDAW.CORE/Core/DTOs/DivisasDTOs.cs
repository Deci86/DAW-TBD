namespace SmallChangeDAW.CORE.Core.DTOs;

public class CambioMonedaRequestDTO
{
    public string MonedaIn { get; set; } = string.Empty;
    public string MonedaOut { get; set; } = string.Empty;
    public decimal Monto { get; set; }
}

public class CambioMonedaResponseDTO
{
    public string MonedaIn { get; set; } = string.Empty;
    public string MonedaOut { get; set; } = string.Empty;
    public decimal TipoCambio { get; set; }
    public decimal Monto { get; set; }
    public decimal MontoConvertido { get; set; }
    public DateTime FechaActualizacion { get; set; }
}

public class TipoCambioResponseDTO
{
    public string MonedaIn { get; set; } = string.Empty;
    public string MonedaOut { get; set; } = string.Empty;
    public decimal TipoCambio { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
