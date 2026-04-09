using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Models;
using ConnectDB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<CategoryDto>> GetAsync(PagedQuery query)
        {
            var q = _repo.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(c => c.CategoryCode.Contains(s) || c.CategoryName.Contains(s));
            }

            q = ApplySort(q, query.SortBy, query.SortDir);

            var total = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryCode = c.CategoryCode,
                    CategoryName = c.CategoryName,
                    Description = c.Description
                })
                .ToListAsync();

            return new PagedResult<CategoryDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new CategoryDto
            {
                Id = entity.Id,
                CategoryCode = entity.CategoryCode,
                CategoryName = entity.CategoryName,
                Description = entity.Description
            };
        }

        public async Task<(CategoryDto? dto, string? error)> CreateAsync(CategoryCreateDto dto)
        {
            if (await _repo.ExistsCodeAsync(dto.CategoryCode))
            {
                return (null, "CategoryCode already exists.");
            }

            var entity = new Category
            {
                CategoryCode = dto.CategoryCode,
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return (new CategoryDto
            {
                Id = entity.Id,
                CategoryCode = entity.CategoryCode,
                CategoryName = entity.CategoryName,
                Description = entity.Description
            }, null);
        }

        public async Task<string?> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            if (id != dto.Id) return "Id mismatch.";
            if (await _repo.ExistsCodeAsync(dto.CategoryCode, dto.Id))
            {
                return "CategoryCode already exists.";
            }

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            entity.CategoryCode = dto.CategoryCode;
            entity.CategoryName = dto.CategoryName;
            entity.Description = dto.Description;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return null;
        }

        public async Task<string?> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            _repo.Remove(entity);
            await _repo.SaveChangesAsync();
            return null;
        }

        private static IQueryable<Category> ApplySort(IQueryable<Category> query, string? sortBy, string? sortDir)
        {
            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? string.Empty).ToLowerInvariant() switch
            {
                "code" => desc ? query.OrderByDescending(c => c.CategoryCode) : query.OrderBy(c => c.CategoryCode),
                "name" => desc ? query.OrderByDescending(c => c.CategoryName) : query.OrderBy(c => c.CategoryName),
                _ => desc ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id)
            };
        }
    }
}
