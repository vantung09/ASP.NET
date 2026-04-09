using ConnectDB.Dtos;
using ConnectDB.Helpers;

namespace ConnectDB.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAsync(PagedQuery query);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<(ProductDto? dto, string? error)> CreateAsync(ProductCreateDto dto);
        Task<string?> UpdateAsync(int id, ProductUpdateDto dto);
        Task<string?> DeleteAsync(int id);
    }
}
