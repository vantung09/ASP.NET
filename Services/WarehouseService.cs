using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Models;
using ConnectDB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _repo;

        public WarehouseService(IWarehouseRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<WarehouseDto>> GetAsync(PagedQuery query)
        {
            var q = _repo.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(w => w.WarehouseCode.Contains(s) || w.WarehouseName.Contains(s));
            }

            q = ApplySort(q, query.SortBy, query.SortDir);

            var total = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(w => new WarehouseDto
                {
                    Id = w.Id,
                    WarehouseCode = w.WarehouseCode,
                    WarehouseName = w.WarehouseName,
                    Address = w.Address
                })
                .ToListAsync();

            return new PagedResult<WarehouseDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<WarehouseDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new WarehouseDto
            {
                Id = entity.Id,
                WarehouseCode = entity.WarehouseCode,
                WarehouseName = entity.WarehouseName,
                Address = entity.Address
            };
        }

        public async Task<(WarehouseDto? dto, string? error)> CreateAsync(WarehouseCreateDto dto)
        {
            if (await _repo.ExistsCodeAsync(dto.WarehouseCode))
            {
                return (null, "WarehouseCode already exists.");
            }

            var entity = new Warehouse
            {
                WarehouseCode = dto.WarehouseCode,
                WarehouseName = dto.WarehouseName,
                Address = dto.Address
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return (new WarehouseDto
            {
                Id = entity.Id,
                WarehouseCode = entity.WarehouseCode,
                WarehouseName = entity.WarehouseName,
                Address = entity.Address
            }, null);
        }

        public async Task<string?> UpdateAsync(int id, WarehouseUpdateDto dto)
        {
            if (id != dto.Id) return "Id mismatch.";
            if (await _repo.ExistsCodeAsync(dto.WarehouseCode, dto.Id))
            {
                return "WarehouseCode already exists.";
            }

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            entity.WarehouseCode = dto.WarehouseCode;
            entity.WarehouseName = dto.WarehouseName;
            entity.Address = dto.Address;

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

        private static IQueryable<Warehouse> ApplySort(IQueryable<Warehouse> query, string? sortBy, string? sortDir)
        {
            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? string.Empty).ToLowerInvariant() switch
            {
                "code" => desc ? query.OrderByDescending(w => w.WarehouseCode) : query.OrderBy(w => w.WarehouseCode),
                "name" => desc ? query.OrderByDescending(w => w.WarehouseName) : query.OrderBy(w => w.WarehouseName),
                _ => desc ? query.OrderByDescending(w => w.Id) : query.OrderBy(w => w.Id)
            };
        }
    }
}
