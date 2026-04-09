using ConnectDB.Dtos;
using ConnectDB.Helpers;

namespace ConnectDB.Services
{
    public interface IWarehouseService
    {
        Task<PagedResult<WarehouseDto>> GetAsync(PagedQuery query);
        Task<WarehouseDto?> GetByIdAsync(int id);
        Task<(WarehouseDto? dto, string? error)> CreateAsync(WarehouseCreateDto dto);
        Task<string?> UpdateAsync(int id, WarehouseUpdateDto dto);
        Task<string?> DeleteAsync(int id);
    }
}
