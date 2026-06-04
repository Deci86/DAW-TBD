namespace SmallChangeDAW.Core.DTOs;

public class CreateOfertaDTO
{
    public int ClienteId { get; set; }
    public string MonedaAEnviar { get; set; } = string.Empty;
    public string MonedaARecibir { get; set; } = string.Empty;
    public decimal TipoCambio { get; set; }
}

public class UpdateOfertaDTO
{
    public int? ClienteId { get; set; }
    public string? MonedaAEnviar { get; set; }
    public string? MonedaARecibir { get; set; }
    public decimal? TipoCambio { get; set; }
    public bool? Estado { get; set; }
}

public class OfertaResponseDTO
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string MonedaAEnviar { get; set; } = string.Empty;
    public string MonedaARecibir { get; set; } = string.Empty;
    public decimal TipoCambio { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}
