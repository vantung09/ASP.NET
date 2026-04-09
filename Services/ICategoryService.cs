using ConnectDB.Dtos;
using ConnectDB.Helpers;

namespace ConnectDB.Services
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDto>> GetAsync(PagedQuery query);
        Task<CategoryDto?> GetByIdAsync(int id);
        Task<(CategoryDto? dto, string? error)> CreateAsync(CategoryCreateDto dto);
        Task<string?> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<string?> DeleteAsync(int id);
    }
}
