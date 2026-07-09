using System;

namespace SmallChangeDAW.CORE.Models;

public class AuditoriaTransaccion
{
    public int id { get; set; }
    public int transaccion_id { get; set; }
    public int usuario_id { get; set; }
    public string accion { get; set; } // 'CREAR', 'ACTUALIZAR_ESTADO', 'ELIMINAR'
    public string? estado_anterior { get; set; }
    public string estado_nuevo { get; set; }
    public DateTime fecha_accion { get; set; }

    // Relaciones
    public Transaccion? Transaccion { get; set; }
    public Cliente? Usuario { get; set; }
}