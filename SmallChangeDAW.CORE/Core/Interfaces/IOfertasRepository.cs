using SmallChangeDAW.Models;

namespace SmallChangeDAW.Core.Interfaces;

public interface IOfertasRepository
{
    Task<IEnumerable<Oferta>> GetAllAsync();
    Task<Oferta?> GetByIdAsync(int id);
    Task<int> AddAsync(Oferta oferta);
    Task<bool> UpdateAsync(Oferta oferta);
    Task<bool> DeleteAsync(int id);
}
