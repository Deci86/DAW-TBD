using SmallChangeDAW.CORE.Core.DTOs;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface ITransaccionesService
{
    Task<IEnumerable<TransaccionResponseDTO>> GetAllAsync();
    Task<TransaccionResponseDTO?> GetByIdAsync(int id);
    Task<TransaccionResponseDTO> AddAsync(CreateTransaccionDTO createDto);
    Task<bool> UpdateAsync(int id, UpdateTransaccionDTO updateDto);
    Task<bool> DeleteAsync(int id);
}
