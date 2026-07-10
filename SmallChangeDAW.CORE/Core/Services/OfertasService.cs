using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<OfertaResponseDTO>> GetByUserIdAsync(int clienteId)
    {
        // Llamamos al repositorio para obtener solo las ofertas de este cliente
        var ofertas = await _ofertasRepository.GetByClienteIdAsync(clienteId);

        // Reutilizamos tu método privado para mapear la lista a DTOs
        return ofertas.Select(MapToDTO);
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

    public async Task<CoincidenciaOfertaResponseDTO?> BuscarCoincidenciaInversaAsync(BuscarCoincidenciaRequestDTO request)
    {
        // 1. Regla matemática básica: Tasa Inversa = 1 / Tasa Actual
        if (request.TipoCambio <= 0) return null;
        decimal tasaInversaTeorica = 1 / request.TipoCambio;

        // 2. Definimos una tolerancia del 2% (0.02) arriba y abajo para la tasa del mercado cruzado
        decimal tolerancia = 0.02m;
        decimal tasaMinima = tasaInversaTeorica * (1 - tolerancia);
        decimal tasaMaxima = tasaInversaTeorica * (1 + tolerancia);

        // 3. Cantidad que el usuario actual espera recibir en la moneda de destino
        decimal cantidadObjetivoADemandar = request.Cantidad * request.TipoCambio;

        // 4. Solicitamos los candidatos crudos al repositorio
        var ofertasCandidatas = await _ofertasRepository.ObtenerOfertasInversasDisponiblesAsync(
            request.MonedaAEnviar,
            request.MonedaARecibir,
            tasaMinima,
            tasaMaxima
        );

        // 5. Procesamos el "Match" óptimo ordenando por la que más se acerque a la cantidad necesitada
        var mejorMatch = ofertasCandidatas
            .OrderBy(o => Math.Abs(o.cantidad - cantidadObjetivoADemandar))
            .FirstOrDefault();

        if (mejorMatch == null) return null;

        // 6. Mapeamos al DTO de salida
        return new CoincidenciaOfertaResponseDTO
        {
            Id = mejorMatch.id,
            Cantidad = mejorMatch.cantidad,
            TipoCambio = mejorMatch.tipo_cambio,
            NombreUsuario = mejorMatch.Cliente?.nombre ?? "Usuario Anónimo"
        };
    }

    public async Task<bool> ActualizarEstadoAsync(int id, bool nuevoEstado)
    {
        return await _ofertasRepository.CambiarEstadoAsync(id, nuevoEstado);
    }
}