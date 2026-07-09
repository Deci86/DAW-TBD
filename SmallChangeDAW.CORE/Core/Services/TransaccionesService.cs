using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Services;

public class TransaccionesService : ITransaccionesService
{
    private readonly ITransaccionesRepository _transaccionesRepository;
    private readonly IOfertasRepository _ofertasRepository;
    private readonly IClientesRepository _clientesRepository;
    private readonly IAuditoriaService _auditoriaService;

    public TransaccionesService(IOfertasRepository ofertasRepository, IClientesRepository clientesRepository, ITransaccionesRepository transaccionesRepository, IAuditoriaService auditoriaService)
    {
        _ofertasRepository = ofertasRepository;
        _clientesRepository = clientesRepository;
        _transaccionesRepository = transaccionesRepository;
        _auditoriaService = auditoriaService;
    }

    public async Task<IEnumerable<TransaccionResponseDTO>> GetAllAsync(int userId)
    {
        // El usuario solo puede ver las transacciones donde es el comprador o donde es el vendedor de la oferta/transaccion asociada
        var transacciones = await _transaccionesRepository.GetAllAsync();

        var transaccionesFiltradas = transacciones
            .Where(t => t.cliente_comprador_id == userId || (t.Oferta != null && t.Oferta.cliente_id == userId));

        return transaccionesFiltradas.Select(MapToDTO);
    }

    public async Task<TransaccionResponseDTO?> GetByIdAsync(int id)
    {
        var transaccion = await _transaccionesRepository.GetByIdAsync(id);
        return transaccion is null ? null : MapToDTO(transaccion);
    }

    public async Task<TransaccionResponseDTO> AddAsync(CreateTransaccionDTO createDto, int usuarioId)
    {
        var oferta = await _ofertasRepository.GetByIdAsync(createDto.OfertaId);
        if (oferta is null) throw new KeyNotFoundException("Oferta no existe.");
        if(oferta.estado == false) throw new InvalidOperationException("La oferta ya no está disponible.");

        var cliente = await _clientesRepository.GetByIdAsync(createDto.ClienteCompradorId);
        if (cliente is null)
            throw new KeyNotFoundException($"El cliente con ID {createDto.ClienteCompradorId} no existe.");

        var transaccion = new Transaccion
        {
            oferta_id = createDto.OfertaId,
            cliente_comprador_id = createDto.ClienteCompradorId,
            estado = "pendiente"
        };

        transaccion.id = await _transaccionesRepository.AddAsync(transaccion);

        // Registrar auditoría de creación
        await _auditoriaService.RegistrarCreacionAsync(transaccion.id, usuarioId, "pendiente");

        return MapToDTO(transaccion);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTransaccionDTO updateDto, int usuarioId)
    {
        var transaccionExistente = await _transaccionesRepository.GetByIdAsync(id);
        if (transaccionExistente == null) return false;

        var estadoAnterior = transaccionExistente.estado;
        transaccionExistente.estado = updateDto.estado;

        var result = await _transaccionesRepository.UpdateAsync(transaccionExistente);

        if (result)
        {
            // Registrar auditoría de cambio de estado
            await _auditoriaService.RegistrarCambioEstadoAsync(id, usuarioId, estadoAnterior, updateDto.estado);
        }

        return result;
    }

    public async Task<bool> DeleteAsync(int id, int usuarioId)
    {
        var transaccion = await _transaccionesRepository.GetByIdAsync(id);
        if (transaccion == null) return false;

        var estadoAnterior = transaccion.estado;
        var result = await _transaccionesRepository.DeleteAsync(id);

        if (result)
        {
            // Registrar auditoría de eliminación
            await _auditoriaService.RegistrarEliminacionAsync(id, usuarioId, estadoAnterior);
        }

        return result;
    }


    private static TransaccionResponseDTO MapToDTO(Transaccion transaccion)
    {
        return new TransaccionResponseDTO
        {
            Id = transaccion.id,
            OfertaId = transaccion.oferta_id,
            ClienteCompradorId = transaccion.cliente_comprador_id,
            estado = transaccion.estado,
            FechaCreacion = transaccion.fecha_transaccion
        };
    }
}
