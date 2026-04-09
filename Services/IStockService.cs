using ConnectDB.Dtos;
using ConnectDB.Helpers;

namespace ConnectDB.Services
{
    public interface IStockService
    {
        Task<PagedResult<StockDto>> GetAsync(PagedQuery query);
        Task<StockDto?> GetAsync(int warehouseId, int productId);
        Task<(StockDto? dto, string? error)> UpsertAsync(StockUpsertDto dto);
        Task<string?> DeleteAsync(int warehouseId, int productId);
    }
}
