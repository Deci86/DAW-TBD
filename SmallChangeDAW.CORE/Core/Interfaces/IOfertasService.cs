using SmallChangeDAW.Core.DTOs;

namespace SmallChangeDAW.Core.Interfaces;

public interface IOfertasService
{
    Task<IEnumerable<OfertaResponseDTO>> GetAllAsync();
    Task<OfertaResponseDTO?> GetByIdAsync(int id);
    Task<OfertaResponseDTO> AddAsync(CreateOfertaDTO createDto);
    Task<bool> UpdateAsync(int id, UpdateOfertaDTO updateDto);
    Task<bool> DeleteAsync(int id);
}
