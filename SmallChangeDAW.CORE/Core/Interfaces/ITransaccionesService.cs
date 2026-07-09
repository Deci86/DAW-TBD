using SmallChangeDAW.CORE.Core.DTOs;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface ITransaccionesService
{
    Task<IEnumerable<TransaccionResponseDTO>> GetAllAsync(int userId);
    Task<TransaccionResponseDTO?> GetByIdAsync(int id);
    Task<TransaccionResponseDTO> AddAsync(CreateTransaccionDTO createDto, int usuarioId);
    Task<bool> UpdateAsync(int id, UpdateTransaccionDTO updateDto, int usuarioId);
    Task<bool> DeleteAsync(int id, int usuarioId);
}
