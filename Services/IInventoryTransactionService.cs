using ConnectDB.Dtos;
using ConnectDB.Helpers;

namespace ConnectDB.Services
{
    public interface IInventoryTransactionService
    {
        Task<PagedResult<InventoryTransactionDto>> GetAsync(PagedQuery query);
        Task<InventoryTransactionDto?> GetByIdAsync(int id);
        Task<(InventoryTransactionDto? dto, string? error)> CreateAsync(InventoryTransactionCreateDto dto);
        Task<string?> UpdateAsync(int id, InventoryTransactionUpdateDto dto);
        Task<string?> DeleteAsync(int id);
    }
}
