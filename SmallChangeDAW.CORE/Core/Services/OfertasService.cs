using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Services;

public class OfertasService : IOfertasService
{
    private readonly IOfertasRepository _ofertasRepository;
    private readonly IClientesRepository _clientesRepository;

    public OfertasService(IOfertasRepository ofertasRepository, IClientesRepository clientesRepository)
    {
        _ofertasRepository = ofertasRepository;
        _clientesRepository = clientesRepository;
    }

    public async Task<IEnumerable<OfertaResponseDTO>> GetAllAsync()
    {
        var ofertas = await _ofertasRepository.GetAllAsync();
        return ofertas.Select(MapToDTO);
    }

    public async Task<OfertaResponseDTO?> GetByIdAsync(int id)
    {
        var oferta = await _ofertasRepository.GetByIdAsync(id);
        return oferta is null ? null : MapToDTO(oferta);
    }

    public async Task<OfertaResponseDTO> AddAsync(CreateOfertaDTO createDto, int clienteId)
    {
        var cliente = await _clientesRepository.GetByIdAsync(clienteId);
        if (cliente is null)
            throw new KeyNotFoundException($"El cliente con ID {clienteId} no existe.");

        var oferta = new Oferta
        {
            cliente_id = clienteId,
            moneda_a_enviar = createDto.MonedaAEnviar,
            moneda_a_recibir = createDto.MonedaARecibir,
            cantidad = createDto.Cantidad,
            tipo_cambio = createDto.TipoCambio,
            estado = true,
            fecha_creacion = DateTime.UtcNow
        };

        oferta.id = await _ofertasRepository.AddAsync(oferta);
        return MapToDTO(oferta);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOfertaDTO updateDto)
    {
        var ofertaExistente = await _ofertasRepository.GetByIdAsync(id);
        if (ofertaExistente == null) return false;

        if (updateDto.MonedaAEnviar != null)
            ofertaExistente.moneda_a_enviar = updateDto.MonedaAEnviar;

        if (updateDto.MonedaARecibir != null)
            ofertaExistente.moneda_a_recibir = updateDto.MonedaARecibir;

        if (updateDto.Cantidad != null)
            ofertaExistente.cantidad = updateDto.Cantidad.Value;

        if (updateDto.TipoCambio != null)
            ofertaExistente.tipo_cambio = updateDto.TipoCambio.Value;

        if (updateDto.Estado != null)
            ofertaExistente.estado = updateDto.Estado.Value;

        return await _ofertasRepository.UpdateAsync(ofertaExistente);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _ofertasRepository.DeleteAsync(id);
    }

    private static OfertaResponseDTO MapToDTO(Oferta oferta)
    {
        return new OfertaResponseDTO
        {
            Id = oferta.id,
            ClienteId = oferta.cliente_id,
            NombreUsuario = oferta.Cliente != null ? oferta.Cliente.nombre : $"Usuario #{oferta.cliente_id}",
            CalificacionUsuario = oferta.Cliente != null ? (double)oferta.Cliente.calificacion_vendedor : 0.0,
            MonedaAEnviar = oferta.moneda_a_enviar,
            MonedaARecibir = oferta.moneda_a_recibir,
            Cantidad = oferta.cantidad,
            TipoCambio = oferta.tipo_cambio,
            Estado = oferta.estado,
            FechaCreacion = oferta.fecha_creacion
        };
    }
}