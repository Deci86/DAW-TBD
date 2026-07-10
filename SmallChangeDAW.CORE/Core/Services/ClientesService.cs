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
            calificacion_vendedor = 0.00m,
            cant_calificaciones = 0
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
        if (updateDto.CalificacionVendedor is not null)
            existing.calificacion_vendedor = updateDto.CalificacionVendedor.Value;
        if (updateDto.CantCalificaciones is not null)
            existing.cant_calificaciones = updateDto.CantCalificaciones.Value;

        return await _clientesRepository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _clientesRepository.DeleteAsync(id);
    }

    public async Task<bool> CalificarUsuarioAsync(int id, decimal calificacionRecibida)
    {
        // 1. Buscamos el cliente (¡Ojo! Asegúrate que el repositorio devuelva una instancia editable, no un clon desconectado)
        var cliente = await _clientesRepository.GetByIdAsync(id);
        if (cliente == null) return false;

        // 2. Definimos las variables de tu ecuación matemática
        int m = cliente.cant_calificaciones + 1; // Nueva cantidad de calificaciones
        decimal n = cliente.calificacion_vendedor; // Promedio actual
        decimal x = calificacionRecibida;          // Calificación entrante

        // 3. Resolvemos la operación matemática asignando el nuevo promedio
        decimal nuevoPromedio = ((m - 1) * n + x) / m;

        // 4. Actualizamos el registro de la entidad
        cliente.cant_calificaciones = m;
        cliente.calificacion_vendedor = Math.Round(nuevoPromedio, 2); // Redondeo estándar a 2 decimales para DB

        // 5. Persistimos los cambios a través del repositorio
        return await _clientesRepository.UpdateAsync(cliente);
    }

    private static ClienteResponseDTO MapToDTO(Cliente cliente)
    {
        return new ClienteResponseDTO
        {
            Id = cliente.id,
            Nombre = cliente.nombre,
            Email = cliente.email,
            CalificacionVendedor = cliente.calificacion_vendedor,
            CantCalificaciones = cliente.cant_calificaciones,
            FechaRegistro = cliente.fecha_registro
        };
    }
}
