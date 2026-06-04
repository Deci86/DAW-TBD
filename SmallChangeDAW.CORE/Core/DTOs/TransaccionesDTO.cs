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
}
