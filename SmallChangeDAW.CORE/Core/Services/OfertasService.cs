using SmallChangeDAW.Core.DTOs;
using SmallChangeDAW.Core.Interfaces;
using SmallChangeDAW.Models;

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

    public async Task<OfertaResponseDTO> AddAsync(CreateOfertaDTO createDto)
    {
        var cliente = await _clientesRepository.GetByIdAsync(createDto.ClienteId);
        if (cliente is null)
            throw new KeyNotFoundException($"El cliente con ID {createDto.ClienteId} no existe.");

        var oferta = new Oferta
        {
            ClienteId = createDto.ClienteId,
            MonedaAEnviar = createDto.MonedaAEnviar,
            MonedaARecibir = createDto.MonedaARecibir,
            TipoCambio = createDto.TipoCambio,
            Estado = true
        };

        oferta.Id = await _ofertasRepository.AddAsync(oferta);
        return MapToDTO(oferta);
    }

    public async Task<bool> UpdateAsync(int id, UpdateOfertaDTO updateDto)
    {
        var existing = await _ofertasRepository.GetByIdAsync(id);
        if (existing is null)
            return false;

        if (updateDto.ClienteId is not null)
        {
            var cliente = await _clientesRepository.GetByIdAsync(updateDto.ClienteId.Value);
            if (cliente is null)
                throw new KeyNotFoundException($"El cliente con ID {updateDto.ClienteId} no existe.");
            existing.ClienteId = updateDto.ClienteId.Value;
        }
        if (updateDto.MonedaAEnviar is not null)
            existing.MonedaAEnviar = updateDto.MonedaAEnviar;
        if (updateDto.MonedaARecibir is not null)
            existing.MonedaARecibir = updateDto.MonedaARecibir;
        if (updateDto.TipoCambio is not null)
            existing.TipoCambio = updateDto.TipoCambio.Value;
        if (updateDto.Estado is not null)
            existing.Estado = updateDto.Estado.Value;

        return await _ofertasRepository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _ofertasRepository.DeleteAsync(id);
    }

    private static OfertaResponseDTO MapToDTO(Oferta oferta)
    {
        return new OfertaResponseDTO
        {
            Id = oferta.Id,
            ClienteId = oferta.ClienteId,
            MonedaAEnviar = oferta.MonedaAEnviar,
            MonedaARecibir = oferta.MonedaARecibir,
            TipoCambio = oferta.TipoCambio,
            Estado = oferta.Estado,
            FechaCreacion = oferta.FechaCreacion
        };
    }
}
