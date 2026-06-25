using Microsoft.EntityFrameworkCore;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Infrastructure.Repositories;


public class ClientesRepository : IClientesRepository
{
    private readonly SmallChangeDbContext _context;

    public ClientesRepository(SmallChangeDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cliente>> GetAllAsync()
    {
        return await _context.Clientes
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.id == id);
    }

    public async Task<int> AddAsync(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return cliente.id;
    }

    public async Task<bool> UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null)
            return false;

        _context.Clientes.Remove(cliente);
        var rowsAffected = await _context.SaveChangesAsync();
        return rowsAffected > 0;
    }
    public async Task<Cliente?> GetByEmailAsync(string email)
    {
        // Busca el primer cliente que coincida con el email, o devuelve null si no existe
        return await _context.Clientes.FirstOrDefaultAsync(c => c.email == email);
    }
}
