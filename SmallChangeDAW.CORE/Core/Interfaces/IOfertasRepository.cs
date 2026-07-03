using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface IOfertasRepository
{
    Task<IEnumerable<Oferta>> GetAllAsync();
    Task<Oferta?> GetByIdAsync(int id);
    Task<IEnumerable<Oferta>> GetByClienteIdAsync(int clienteId);
    Task<int> AddAsync(Oferta oferta);
    Task<bool> UpdateAsync(Oferta oferta);
    Task<bool> DeleteAsync(int id);
}
