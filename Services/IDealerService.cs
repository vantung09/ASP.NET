using ConnectDB.Dtos;
using ConnectDB.Helpers;

namespace ConnectDB.Services
{
    public interface IDealerService
    {
        Task<PagedResult<DealerDto>> GetAsync(PagedQuery query);
        Task<DealerDto?> GetByIdAsync(int id);
        Task<(DealerDto? dto, string? error)> CreateAsync(DealerCreateDto dto);
        Task<string?> UpdateAsync(int id, DealerUpdateDto dto);
        Task<string?> DeleteAsync(int id);
    }
}
