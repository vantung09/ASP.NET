using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Models;
using ConnectDB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _categoryRepo;

        public ProductService(IProductRepository repo, ICategoryRepository categoryRepo)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
        }

        public async Task<PagedResult<ProductDto>> GetAsync(PagedQuery query)
        {
            var q = _repo.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(p => p.ProductCode.Contains(s) || p.ProductName.Contains(s));
            }

            q = ApplySort(q, query.SortBy, query.SortDir);

            var total = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Unit = p.Unit,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new ProductDto
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode,
                ProductName = entity.ProductName,
                Description = entity.Description,
                Unit = entity.Unit,
                Price = entity.Price,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category != null ? entity.Category.CategoryName : string.Empty
            };
        }

        public async Task<(ProductDto? dto, string? error)> CreateAsync(ProductCreateDto dto)
        {
            if (await _repo.ExistsCodeAsync(dto.ProductCode))
            {
                return (null, "ProductCode already exists.");
            }

            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return (null, "Category not found.");
            }

            var entity = new Product
            {
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                Description = dto.Description,
                Unit = dto.Unit,
                Price = dto.Price,
                CategoryId = dto.CategoryId
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return (new ProductDto
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode,
                ProductName = entity.ProductName,
                Description = entity.Description,
                Unit = entity.Unit,
                Price = entity.Price,
                CategoryId = entity.CategoryId,
                CategoryName = category.CategoryName
            }, null);
        }

        public async Task<string?> UpdateAsync(int id, ProductUpdateDto dto)
        {
            if (id != dto.Id) return "Id mismatch.";
            if (await _repo.ExistsCodeAsync(dto.ProductCode, dto.Id))
            {
                return "ProductCode already exists.";
            }

            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return "Category not found.";
            }

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            entity.ProductCode = dto.ProductCode;
            entity.ProductName = dto.ProductName;
            entity.Description = dto.Description;
            entity.Unit = dto.Unit;
            entity.Price = dto.Price;
            entity.CategoryId = dto.CategoryId;

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

        private static IQueryable<Product> ApplySort(IQueryable<Product> query, string? sortBy, string? sortDir)
        {
            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? string.Empty).ToLowerInvariant() switch
            {
                "code" => desc ? query.OrderByDescending(p => p.ProductCode) : query.OrderBy(p => p.ProductCode),
                "name" => desc ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
                "price" => desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                _ => desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
            };
        }
    }
}
