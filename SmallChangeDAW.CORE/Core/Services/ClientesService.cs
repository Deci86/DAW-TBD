using SmallChangeDAW.CORE.Core.DTOs;
using SmallChangeDAW.CORE.Core.Interfaces;
using SmallChangeDAW.CORE.Models;

namespace SmallChangeDAW.CORE.Core.Services;

public class ClientesService : IClientesService
{
    private readonly IClientesRepository _clientesRepository;

    public ClientesService(IClientesRepository clientesRepository)
    {
        _clientesRepository = clientesRepository;
    }

    public async Task<IEnumerable<ClienteResponseDTO>> GetAllAsync()
    {
        var clientes = await _clientesRepository.GetAllAsync();
        return clientes.Select(MapToDTO);
    }

    public async Task<ClienteResponseDTO?> GetByIdAsync(int id)
    {
        var cliente = await _clientesRepository.GetByIdAsync(id);
        return cliente is null ? null : MapToDTO(cliente);
    }

    public async Task<ClienteResponseDTO> AddAsync(CreateClienteDTO createDto)
    {
        var cliente = new Cliente
        {
            nombre = createDto.Nombre,
            email = createDto.Email,
            pass_hash = createDto.PassHash,
            promedio_calificacion_comprador = 0.00m,
            calificacion_vendedor = 0.00m
        };

        cliente.id = await _clientesRepository.AddAsync(cliente);
        return MapToDTO(cliente);
    }

    public async Task<bool> UpdateAsync(int id, UpdateClienteDTO updateDto)
    {
        var existing = await _clientesRepository.GetByIdAsync(id);
        if (existing is null)
            return false;

        if (updateDto.Nombre is not null)
            existing.nombre = updateDto.Nombre;
        if (updateDto.Email is not null)
            existing.email = updateDto.Email;
        if (updateDto.PassHash is not null)
            existing.pass_hash = updateDto.PassHash;
        if (updateDto.PromedioCalificacionComprador is not null)
            existing.promedio_calificacion_comprador = updateDto.PromedioCalificacionComprador.Value;
        if (updateDto.CalificacionVendedor is not null)
            existing.calificacion_vendedor = updateDto.CalificacionVendedor.Value;

        return await _clientesRepository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _clientesRepository.DeleteAsync(id);
    }

    private static ClienteResponseDTO MapToDTO(Cliente cliente)
    {
        return new ClienteResponseDTO
        {
            Id = cliente.id,
            Nombre = cliente.nombre,
            Email = cliente.email,
            PromedioCalificacionComprador = cliente.promedio_calificacion_comprador,
            CalificacionVendedor = cliente.calificacion_vendedor,
            FechaRegistro = cliente.fecha_registro
        };
    }
}
