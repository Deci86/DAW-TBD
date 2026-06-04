namespace SmallChangeDAW.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PassHash { get; set; } = string.Empty;
    public decimal PromedioCalificacionComprador { get; set; }
    public decimal CalificacionVendedor { get; set; }
    public DateTime FechaRegistro { get; set; }
}
