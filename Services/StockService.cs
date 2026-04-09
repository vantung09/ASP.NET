using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Models;
using ConnectDB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _repo;
        private readonly IWarehouseRepository _warehouseRepo;
        private readonly IProductRepository _productRepo;

        public StockService(IStockRepository repo, IWarehouseRepository warehouseRepo, IProductRepository productRepo)
        {
            _repo = repo;
            _warehouseRepo = warehouseRepo;
            _productRepo = productRepo;
        }

        public async Task<PagedResult<StockDto>> GetAsync(PagedQuery query)
        {
            var q = _repo.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(ws =>
                    ws.Product != null && (ws.Product.ProductCode.Contains(s) || ws.Product.ProductName.Contains(s)) ||
                    ws.Warehouse != null && (ws.Warehouse.WarehouseCode.Contains(s) || ws.Warehouse.WarehouseName.Contains(s)));
            }

            q = ApplySort(q, query.SortBy, query.SortDir);

            var total = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(ws => new StockDto
                {
                    WarehouseId = ws.WarehouseId,
                    WarehouseCode = ws.Warehouse != null ? ws.Warehouse.WarehouseCode : string.Empty,
                    WarehouseName = ws.Warehouse != null ? ws.Warehouse.WarehouseName : string.Empty,
                    ProductId = ws.ProductId,
                    ProductCode = ws.Product != null ? ws.Product.ProductCode : string.Empty,
                    ProductName = ws.Product != null ? ws.Product.ProductName : string.Empty,
                    Quantity = ws.Quantity,
                    UpdatedAt = ws.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<StockDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<StockDto?> GetAsync(int warehouseId, int productId)
        {
            var entity = await _repo.GetAsync(warehouseId, productId);
            if (entity == null) return null;

            return new StockDto
            {
                WarehouseId = entity.WarehouseId,
                WarehouseCode = entity.Warehouse?.WarehouseCode ?? string.Empty,
                WarehouseName = entity.Warehouse?.WarehouseName ?? string.Empty,
                ProductId = entity.ProductId,
                ProductCode = entity.Product?.ProductCode ?? string.Empty,
                ProductName = entity.Product?.ProductName ?? string.Empty,
                Quantity = entity.Quantity,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public async Task<(StockDto? dto, string? error)> UpsertAsync(StockUpsertDto dto)
        {
            var warehouse = await _warehouseRepo.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null) return (null, "Warehouse not found.");

            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null) return (null, "Product not found.");

            var entity = await _repo.GetAsync(dto.WarehouseId, dto.ProductId);
            if (entity == null)
            {
                entity = new WarehouseStock
                {
                    WarehouseId = dto.WarehouseId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(entity);
            }
            else
            {
                entity.Quantity = dto.Quantity;
                entity.UpdatedAt = DateTime.UtcNow;
                _repo.Update(entity);
            }

            await _repo.SaveChangesAsync();

            return (new StockDto
            {
                WarehouseId = entity.WarehouseId,
                WarehouseCode = warehouse.WarehouseCode,
                WarehouseName = warehouse.WarehouseName,
                ProductId = entity.ProductId,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Quantity = entity.Quantity,
                UpdatedAt = entity.UpdatedAt
            }, null);
        }

        public async Task<string?> DeleteAsync(int warehouseId, int productId)
        {
            var entity = await _repo.GetAsync(warehouseId, productId);
            if (entity == null) return "Not found.";

            _repo.Remove(entity);
            await _repo.SaveChangesAsync();
            return null;
        }

        private static IQueryable<WarehouseStock> ApplySort(IQueryable<WarehouseStock> query, string? sortBy, string? sortDir)
        {
            var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? string.Empty).ToLowerInvariant() switch
            {
                "warehouse" => desc
                    ? query.OrderByDescending(ws => ws.Warehouse != null ? ws.Warehouse.WarehouseName : string.Empty)
                    : query.OrderBy(ws => ws.Warehouse != null ? ws.Warehouse.WarehouseName : string.Empty),
                "product" => desc
                    ? query.OrderByDescending(ws => ws.Product != null ? ws.Product.ProductName : string.Empty)
                    : query.OrderBy(ws => ws.Product != null ? ws.Product.ProductName : string.Empty),
                "quantity" => desc ? query.OrderByDescending(ws => ws.Quantity) : query.OrderBy(ws => ws.Quantity),
                _ => desc ? query.OrderByDescending(ws => ws.UpdatedAt) : query.OrderBy(ws => ws.UpdatedAt)
            };
        }
    }
}
