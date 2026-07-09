using Microsoft.EntityFrameworkCore;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Infrastructure.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly SmallChangeDbContext _context;

    public AuditoriaRepository(SmallChangeDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(AuditoriaTransaccion auditoria)
    {
        _context.AuditoriasTransacciones.Add(auditoria);
        await _context.SaveChangesAsync();
        return auditoria.id;
    }

    public async Task<IEnumerable<AuditoriaTransaccion>> GetByTransaccionIdAsync(int transaccionId)
    {
        return await _context.AuditoriasTransacciones
            .Include(a => a.Usuario)
            .Where(a => a.transaccion_id == transaccionId)
            .OrderByDescending(a => a.fecha_accion)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditoriaTransaccion>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _context.AuditoriasTransacciones
            .Include(a => a.Transaccion)
            .Include(a => a.Usuario)
            .Where(a => a.usuario_id == usuarioId)
            .OrderByDescending(a => a.fecha_accion)
            .AsNoTracking()
            .ToListAsync();
    }
}
