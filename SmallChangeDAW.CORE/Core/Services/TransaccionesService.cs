using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Services;

public class TransaccionesService : ITransaccionesService
{
    private readonly ITransaccionesRepository _transaccionesRepository;
    private readonly IOfertasRepository _ofertasRepository;
    private readonly IClientesRepository _clientesRepository;

    public TransaccionesService(IOfertasRepository ofertasRepository, IClientesRepository clientesRepository, ITransaccionesRepository transaccionesRepository)
    {
        _ofertasRepository = ofertasRepository;
        _clientesRepository = clientesRepository;
        _transaccionesRepository = transaccionesRepository;
    }

    public async Task<IEnumerable<TransaccionResponseDTO>> GetAllAsync()
    {
        var transacciones = await _transaccionesRepository.GetAllAsync();
        return transacciones.Select(MapToDTO);
    }

    public async Task<TransaccionResponseDTO?> GetByIdAsync(int id)
    {
        var transaccion = await _transaccionesRepository.GetByIdAsync(id);
        return transaccion is null ? null : MapToDTO(transaccion);
    }

    public async Task<TransaccionResponseDTO> AddAsync(CreateTransaccionDTO createDto)
    {
        var cliente = await _clientesRepository.GetByIdAsync(createDto.ClienteCompradorId);
        if (cliente is null)
            throw new KeyNotFoundException($"El cliente con ID {createDto.ClienteCompradorId} no existe.");

        var transaccion = new Transaccion
        {
            cliente_comprador_id = createDto.ClienteCompradorId,
            oferta_id = createDto.OfertaId,
            estado = "pendiente"
        };

        transaccion.id = await _transaccionesRepository.AddAsync(transaccion);
        return MapToDTO(transaccion);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTransaccionDTO updateDto)
    {
        var transaccionExistente = await _transaccionesRepository.GetByIdAsync(id);
        if (transaccionExistente == null) return false;

        transaccionExistente.estado = updateDto.estado;

        return await _transaccionesRepository.UpdateAsync(transaccionExistente);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _transaccionesRepository.DeleteAsync(id);
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
