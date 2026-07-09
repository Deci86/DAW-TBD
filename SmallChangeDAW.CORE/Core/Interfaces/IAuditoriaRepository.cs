using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface IAuditoriaRepository
{
    Task<int> AddAsync(AuditoriaTransaccion auditoria);
    Task<IEnumerable<AuditoriaTransaccion>> GetByTransaccionIdAsync(int transaccionId);
    Task<IEnumerable<AuditoriaTransaccion>> GetByUsuarioIdAsync(int usuarioId);
}
