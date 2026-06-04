namespace SmallChangeDAW.CORE.Models;

public class Transaccion
{
    public int id { get; set; }
    public int oferta_id { get; set; }
    public int cliente_comprador_id { get; set; }
    public DateTime fecha_transaccion { get; set; }
    public string estado { get; set; }
}