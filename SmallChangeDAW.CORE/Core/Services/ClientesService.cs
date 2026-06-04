using SmallChangeDAW.Core.DTOs;
using SmallChangeDAW.Core.Interfaces;
using SmallChangeDAW.Models;

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
            Nombre = createDto.Nombre,
            Email = createDto.Email,
            PassHash = createDto.PassHash,
            PromedioCalificacionComprador = 0.00m,
            CalificacionVendedor = 0.00m
        };

        cliente.Id = await _clientesRepository.AddAsync(cliente);
        return MapToDTO(cliente);
    }

    public async Task<bool> UpdateAsync(int id, UpdateClienteDTO updateDto)
    {
        var existing = await _clientesRepository.GetByIdAsync(id);
        if (existing is null)
            return false;

        if (updateDto.Nombre is not null)
            existing.Nombre = updateDto.Nombre;
        if (updateDto.Email is not null)
            existing.Email = updateDto.Email;
        if (updateDto.PassHash is not null)
            existing.PassHash = updateDto.PassHash;
        if (updateDto.PromedioCalificacionComprador is not null)
            existing.PromedioCalificacionComprador = updateDto.PromedioCalificacionComprador.Value;
        if (updateDto.CalificacionVendedor is not null)
            existing.CalificacionVendedor = updateDto.CalificacionVendedor.Value;

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
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Email = cliente.Email,
            PromedioCalificacionComprador = cliente.PromedioCalificacionComprador,
            CalificacionVendedor = cliente.CalificacionVendedor,
            FechaRegistro = cliente.FechaRegistro
        };
    }
}
