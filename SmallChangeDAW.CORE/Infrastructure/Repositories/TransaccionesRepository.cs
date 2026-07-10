using Microsoft.EntityFrameworkCore;
using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Infrastructure.Data;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Infrastructure.Repositories;

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
            .Include(t => t.Oferta)
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

    public async Task<Transaccion> CrearConDisputaAsync(CreateTransaccionDTO createDto, int usuarioIdActual)
    {
        // Iniciamos la transacción a nivel de Base de Datos mediante EF Core
        using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Obtener la oferta bloqueando lógicamente su estado
            var oferta = await _context.Ofertas
                .FirstOrDefaultAsync(o => o.id == createDto.OfertaId);

            if (oferta == null)
                throw new KeyNotFoundException("La oferta ya no existe en el mercado.");

            if (oferta.estado == false)
                throw new InvalidOperationException("La oferta ya no está disponible.");

            // 2. Verificar si existe otra transacción en estado 'pendiente' compitiendo por la misma oferta
            var transaccionCompetidora = await _context.Transacciones
                .Include(t => t.ClienteComprador)
                .FirstOrDefaultAsync(t => t.oferta_id == createDto.OfertaId && t.estado == "pendiente");

            if (transaccionCompetidora != null)
            {
                // Consultamos las propiedades de antigüedad (fecha_registro) de ambos clientes
                var clienteActual = await _context.Clientes.FindAsync(usuarioIdActual);

                if (clienteActual.fecha_registro < transaccionCompetidora.ClienteComprador.fecha_registro)
                {
                    // El usuario actual es más antiguo: se revoca la transacción del competidor anterior
                    transaccionCompetidora.estado = "cancelada";
                    _context.Transacciones.Update(transaccionCompetidora);

                    // Guardamos temporalmente para registrar el cambio de estado del competidor
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // El usuario actual es más nuevo o igual: pierde la disputa de concurrencia
                    throw new InvalidOperationException("Disputa de Compra: Otro usuario con mayor antigüedad en la plataforma ha tomado prioridad sobre esta oferta.");
                }
            }

            // 3. Crear la nueva transacción para el ganador de la disputa
            var nuevaTransaccion = new Transaccion
            {
                oferta_id = createDto.OfertaId, // Mapeado según las propiedades de tu DTO
                cliente_comprador_id = usuarioIdActual,
                estado = "pendiente",
                fecha_transaccion = DateTime.Now
            };

            _context.Transacciones.Add(nuevaTransaccion);
            await _context.SaveChangesAsync();

            // Consolidamos la transacción de manera segura
            await dbTransaction.CommitAsync();

            return nuevaTransaccion;
        }
        catch (Exception)
        {
            // Deshacemos cualquier mutación intermedia si la disputa falló
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
}
