namespace SmallChangeDAW.CORE.Models;

public class Oferta
{
    public int id { get; set; }
    public int cliente_id { get; set; }
    public Cliente? Cliente { get; set; }
    public string moneda_a_enviar { get; set; } = string.Empty;
    public string moneda_a_recibir { get; set; } = string.Empty;
    public decimal cantidad { get; set; }
    public decimal tipo_cambio { get; set; }
    public bool estado { get; set; }
    public DateTime fecha_creacion { get; set; }
}