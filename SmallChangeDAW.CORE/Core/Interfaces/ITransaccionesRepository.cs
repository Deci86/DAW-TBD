using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface ITransaccionesRepository
{
    Task<IEnumerable<Transaccion>> GetAllAsync();
    Task<Transaccion?> GetByIdAsync(int id);
    Task<int> AddAsync(Transaccion transaccion);
    Task<bool> UpdateAsync(Transaccion transaccion);
    Task<bool> DeleteAsync(int id);
}
