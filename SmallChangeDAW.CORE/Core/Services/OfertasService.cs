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
            tipo_cambio = createDto.TipoCambio,
            estado = true
        };

        oferta.id = await _ofertasRepository.AddAsync(oferta);
        return MapToDTO(oferta);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOfertaDTO updateDto)
    {
        var ofertaExistente = await _ofertasRepository.GetByIdAsync(id);
        if (ofertaExistente == null) return false;

        ofertaExistente.cliente_id = updateDto.ClienteId ?? 0;
        ofertaExistente.moneda_a_enviar = updateDto.MonedaAEnviar ?? string.Empty;
        ofertaExistente.moneda_a_recibir = updateDto.MonedaARecibir ?? string.Empty;
        ofertaExistente.tipo_cambio = updateDto.TipoCambio ?? 0m;
        ofertaExistente.estado = updateDto.Estado ?? false;

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
            MonedaAEnviar = oferta.moneda_a_enviar,
            MonedaARecibir = oferta.moneda_a_recibir,
            TipoCambio = oferta.tipo_cambio,
            Estado = oferta.estado,
            FechaCreacion = oferta.fecha_creacion
        };
    }
}
