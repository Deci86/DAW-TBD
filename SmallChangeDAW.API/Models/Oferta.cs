namespace SmallChangeDAW.Models;

public class Oferta
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string MonedaAEnviar { get; set; } = string.Empty;
    public string MonedaARecibir { get; set; } = string.Empty;
    public decimal TipoCambio { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
}
