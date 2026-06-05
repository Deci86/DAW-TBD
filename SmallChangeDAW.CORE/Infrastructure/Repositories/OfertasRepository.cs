using Microsoft.EntityFrameworkCore;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Infrastructure.Repositories;


public class OfertasRepository : IOfertasRepository
{
    private readonly SmallChangeDbContext _context;

    public OfertasRepository(SmallChangeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Oferta>> GetAllAsync()
    {
        return await _context.Ofertas
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Oferta?> GetByIdAsync(int id)
    {
        return await _context.Ofertas
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.id == id);
    }

    public async Task<int> AddAsync(Oferta oferta)
    {
        _context.Ofertas.Add(oferta);
        await _context.SaveChangesAsync();
        return oferta.id;
    }

    public async Task<bool> UpdateAsync(Oferta oferta)
    {
        _context.Ofertas.Update(oferta);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var oferta = await _context.Ofertas.FindAsync(id);
        if (oferta == null)
            return false;

        _context.Ofertas.Remove(oferta);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }
}
