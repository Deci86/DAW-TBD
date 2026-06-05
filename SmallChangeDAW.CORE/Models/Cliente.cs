namespace SmallChangeDAW.CORE.Models;

public class Cliente
{
    public int id { get; set; }
    public string nombre { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string pass_hash { get; set; } = string.Empty;
    public decimal promedio_calificacion_comprador { get; set; }
    public decimal calificacion_vendedor { get; set; }
    public DateTime fecha_registro { get; set; }
}
