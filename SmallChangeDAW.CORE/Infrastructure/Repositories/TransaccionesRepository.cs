using Microsoft.EntityFrameworkCore;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Infrastructure.Repositories;

/// <summary>
/// Repositorio de Transacciones con Entity Framework Core
/// Proporciona acceso seguro y parametrizado a datos de transacciones
/// </summary>
public class TransaccionesRepository : ITransaccionesRepository
{
    private readonly SmallChangeDbContext _context;

    public TransaccionesRepository(SmallChangeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Transaccion>> GetAllAsync()
    {
        return await _context.Transacciones
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Transaccion?> GetByIdAsync(int id)
    {
        return await _context.Transacciones
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.id == id);
    }

    public async Task<int> AddAsync(Transaccion transaccion)
    {
        _context.Transacciones.Add(transaccion);
        await _context.SaveChangesAsync();
        return transaccion.id;
    }

    public async Task<bool> UpdateAsync(Transaccion transaccion)
    {
        _context.Transacciones.Update(transaccion);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transaccion = await _context.Transacciones.FindAsync(id);
        if (transaccion == null)
            return false;

        _context.Transacciones.Remove(transaccion);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }
}
