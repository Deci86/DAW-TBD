using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.DTOs;

public class CreateTransaccionDTO
{
    public int OfertaId { get; set; }
    public int ClienteCompradorId { get; set; }
    public string estado { get; set; }
}

public class UpdateTransaccionDTO
{
    public string? estado { get; set; }
}

public class TransaccionResponseDTO
{
    public int Id { get; set; }
    public int OfertaId { get; set; }
    public int ClienteCompradorId { get; set; }
    public string estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public Oferta? Oferta { get; set; }
    public Cliente? ClienteComprador { get; set; }
}

public class AuditoriaTransaccionResponseDTO
{
    public int Id { get; set; }
    public int TransaccionId { get; set; }
    public int UsuarioId { get; set; }
    public string Accion { get; set; }
    public string? EstadoAnterior { get; set; }
    public string EstadoNuevo { get; set; }
    public DateTime FechaAccion { get; set; }
    public ClienteAuditoriaDTO? Usuario { get; set; }
}

public class ClienteAuditoriaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
}

