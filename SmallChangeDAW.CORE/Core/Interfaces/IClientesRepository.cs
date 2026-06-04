using SmallChangeDAW.Models;

namespace SmallChangeDAW.Core.Interfaces;

public interface IClientesRepository
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<int> AddAsync(Cliente cliente);
    Task<bool> UpdateAsync(Cliente cliente);
    Task<bool> DeleteAsync(int id);
}
