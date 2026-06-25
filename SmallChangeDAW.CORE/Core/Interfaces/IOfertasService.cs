using SmallChangeDAW.CORE.Core.DTOs;

namespace SmallChangeDAW.CORE.Core.Interfaces;

public interface IOfertasService
{
    Task<IEnumerable<OfertaResponseDTO>> GetAllAsync();
    Task<OfertaResponseDTO?> GetByIdAsync(int id);
    Task<OfertaResponseDTO> AddAsync(CreateOfertaDTO createDto, int clienteId);
    Task<bool> UpdateAsync(int id, UpdateOfertaDTO updateDto);
    Task<bool> DeleteAsync(int id);
}
