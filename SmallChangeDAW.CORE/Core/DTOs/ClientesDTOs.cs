namespace SmallChangeDAW.Core.DTOs;

public class CreateClienteDTO
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PassHash { get; set; } = string.Empty;
}

public class UpdateClienteDTO
{
    public string? Nombre { get; set; }
    public string? Email { get; set; }
    public string? PassHash { get; set; }
    public decimal? PromedioCalificacionComprador { get; set; }
    public decimal? CalificacionVendedor { get; set; }
}

public class ClienteResponseDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal PromedioCalificacionComprador { get; set; }
    public decimal CalificacionVendedor { get; set; }
    public DateTime FechaRegistro { get; set; }
}
