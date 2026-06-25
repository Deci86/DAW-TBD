using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface IClientesRepository
{
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente?> GetByIdAsync(int id);
    Task<int> AddAsync(Cliente cliente);
    Task<bool> UpdateAsync(Cliente cliente);
    Task<bool> DeleteAsync(int id); 
    Task<Cliente?> GetByEmailAsync(string email);
}
