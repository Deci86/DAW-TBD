using SmallChangeDAW.Core.DTOs;

namespace SmallChangeDAW.Core.Interfaces;

public interface IClientesService
{
    Task<IEnumerable<ClienteResponseDTO>> GetAllAsync();
    Task<ClienteResponseDTO?> GetByIdAsync(int id);
    Task<ClienteResponseDTO> AddAsync(CreateClienteDTO createDto);
    Task<bool> UpdateAsync(int id, UpdateClienteDTO updateDto);
    Task<bool> DeleteAsync(int id);
}
