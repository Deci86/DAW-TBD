namespace SmallChangeDAW.CORE.Core.DTOs;

public class CreateOfertaDTO
{
    public string MonedaAEnviar { get; set; } = string.Empty;
    public string MonedaARecibir { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal TipoCambio { get; set; }
}

public class UpdateOfertaDTO
{
    public string? MonedaAEnviar { get; set; }
    public string? MonedaARecibir { get; set; }
    public decimal? Cantidad { get; set; }
    public decimal? TipoCambio { get; set; }
    public bool? Estado { get; set; }
}
public class OfertaResponseDTO
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public double CalificacionUsuario { get; set; }
    public string MonedaAEnviar { get; set; } = string.Empty;
    public string MonedaARecibir { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal TipoCambio { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class BuscarCoincidenciaRequestDTO
{
    public string MonedaAEnviar { get; set; } = string.Empty; // Lo que tiene el usuario actual
    public string MonedaARecibir { get; set; } = string.Empty; // Lo que quiere el usuario actual
    public decimal Cantidad { get; set; }
    public decimal TipoCambio { get; set; }
}

public class CoincidenciaOfertaResponseDTO
{
    public int Id { get; set; }
    public decimal Cantidad { get; set; }
    public decimal TipoCambio { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public decimal PorcentajeSimilitud { get; set; } // Opcional: para mostrar qué tan cerca está del match perfecto
}

public class ActualizarEstadoOfertaDTO
{
    public bool Estado { get; set; }
}