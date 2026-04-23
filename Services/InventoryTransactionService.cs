using ConnectDB.Data;
using ConnectDB.Dtos;
using ConnectDB.Helpers;
using ConnectDB.Models;
using ConnectDB.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly AppDbContext _context;
        private readonly IInventoryTransactionRepository _repo;
        private readonly IWarehouseRepository _warehouseRepo;
        private readonly IProductRepository _productRepo;
        private readonly IDealerRepository _dealerRepo;
        private readonly IStockRepository _stockRepo;

        public InventoryTransactionService(
            AppDbContext context,
            IInventoryTransactionRepository repo,
            IWarehouseRepository warehouseRepo,
            IProductRepository productRepo,
            IDealerRepository dealerRepo,
            IStockRepository stockRepo)
        {
            _context = context;
            _repo = repo;
            _warehouseRepo = warehouseRepo;
            _productRepo = productRepo;
            _dealerRepo = dealerRepo;
            _stockRepo = stockRepo;
        }

        public async Task<PagedResult<InventoryTransactionDto>> GetAsync(PagedQuery query)
        {
            var q = _repo.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var s = query.Search.Trim();
                q = q.Where(t =>
                    t.TransactionCode.Contains(s) ||
                    (t.Product != null && (t.Product.ProductCode.Contains(s) || t.Product.ProductName.Contains(s))) ||
                    (t.Warehouse != null && (t.Warehouse.WarehouseCode.Contains(s) || t.Warehouse.WarehouseName.Contains(s))) ||
                    (t.Dealer != null && (t.Dealer.DealerCode.Contains(s) || t.Dealer.DealerName.Contains(s))) ||
                    t.Note.Contains(s));
            }

            q = ApplySort(q, query.SortBy, query.SortDir);

            var total = await q.CountAsync();
            var items = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(t => new InventoryTransactionDto
                {
                    Id = t.Id,
                    TransactionCode = t.TransactionCode,
                    TransactionType = t.TransactionType,
                    WarehouseId = t.WarehouseId,
                    WarehouseCode = t.Warehouse != null ? t.Warehouse.WarehouseCode : string.Empty,
                    WarehouseName = t.Warehouse != null ? t.Warehouse.WarehouseName : string.Empty,
                    ProductId = t.ProductId,
                    ProductCode = t.Product != null ? t.Product.ProductCode : string.Empty,
                    ProductName = t.Product != null ? t.Product.ProductName : string.Empty,
                    DealerId = t.DealerId,
                    DealerCode = t.Dealer != null ? t.Dealer.DealerCode : string.Empty,
                    DealerName = t.Dealer != null ? t.Dealer.DealerName : string.Empty,
                    Quantity = t.Quantity,
                    UnitPrice = t.UnitPrice,
                    TotalAmount = t.UnitPrice * t.Quantity,
                    TransactionDate = t.TransactionDate,
                    Note = t.Note,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<InventoryTransactionDto>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<InventoryTransactionDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<(InventoryTransactionDto? dto, string? error)> CreateAsync(InventoryTransactionCreateDto dto)
        {
            if (await _repo.ExistsCodeAsync(dto.TransactionCode))
            {
                return (null, "TransactionCode already exists.");
            }

            var transactionType = NormalizeTransactionType(dto.TransactionType);
            if (transactionType == null)
            {
                return (null, "TransactionType must be Import or Export.");
            }

            var warehouse = await _warehouseRepo.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null) return (null, "Warehouse not found.");

            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null) return (null, "Product not found.");

            var dealer = await _dealerRepo.GetByIdAsync(dto.DealerId);
            if (dealer == null) return (null, "Dealer not found.");
            if (!DealerSupportsTransaction(dealer.DealerType, transactionType))
            {
                return (null, "Dealer type does not support this transaction.");
            }

            var entity = new InventoryTransaction
            {
                TransactionCode = dto.TransactionCode,
                TransactionType = transactionType,
                WarehouseId = dto.WarehouseId,
                ProductId = dto.ProductId,
                DealerId = dto.DealerId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TransactionDate = dto.TransactionDate == default ? DateTime.UtcNow : dto.TransactionDate,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var stockError = await ApplyStockImpactAsync(entity.TransactionType, entity.WarehouseId, entity.ProductId, entity.Quantity);
            if (stockError != null)
            {
                return (null, stockError);
            }

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            await transaction.CommitAsync();

            entity.Warehouse = warehouse;
            entity.Product = product;
            entity.Dealer = dealer;

            return (ToDto(entity), null);
        }

        public async Task<string?> UpdateAsync(int id, InventoryTransactionUpdateDto dto)
        {
            if (id != dto.Id) return "Id mismatch.";
            if (await _repo.ExistsCodeAsync(dto.TransactionCode, dto.Id))
            {
                return "TransactionCode already exists.";
            }

            var transactionType = NormalizeTransactionType(dto.TransactionType);
            if (transactionType == null)
            {
                return "TransactionType must be Import or Export.";
            }

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            var warehouse = await _warehouseRepo.GetByIdAsync(dto.WarehouseId);
            if (warehouse == null) return "Warehouse not found.";

            var product = await _productRepo.GetByIdAsync(dto.ProductId);
            if (product == null) return "Product not found.";

            var dealer = await _dealerRepo.GetByIdAsync(dto.DealerId);
            if (dealer == null) return "Dealer not found.";
            if (!DealerSupportsTransaction(dealer.DealerType, transactionType))
            {
                return "Dealer type does not support this transaction.";
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var revertError = await RevertStockImpactAsync(entity);
            if (revertError != null)
            {
                return revertError;
            }

            var applyError = await ApplyStockImpactAsync(transactionType, dto.WarehouseId, dto.ProductId, dto.Quantity);
            if (applyError != null)
            {
                return applyError;
            }

            entity.TransactionCode = dto.TransactionCode;
            entity.TransactionType = transactionType;
            entity.WarehouseId = dto.WarehouseId;
            entity.ProductId = dto.ProductId;
            entity.DealerId = dto.DealerId;
            entity.Quantity = dto.Quantity;
            entity.UnitPrice = dto.UnitPrice;
            entity.TransactionDate = dto.TransactionDate == default ? entity.TransactionDate : dto.TransactionDate;
            entity.Note = dto.Note;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            await transaction.CommitAsync();
            return null;
        }

        public async Task<string?> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return "Not found.";

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var revertError = await RevertStockImpactAsync(entity);
            if (revertError != null)
            {
                return revertError;
            }

            _repo.Remove(entity);
            await _repo.SaveChangesAsync();
            await transaction.CommitAsync();
            return null;
        }

        private async Task<string?> ApplyStockImpactAsync(string transactionType, int warehouseId, int productId, int quantity)
        {
            var delta = transactionType == "Import" ? quantity : -quantity;
            return await AdjustStockAsync(warehouseId, productId, delta, transactionType);
        }

        private async Task<string?> RevertStockImpactAsync(InventoryTransaction entity)
        {
            var delta = entity.TransactionType == "Import" ? -entity.Quantity : entity.Quantity;
            return await AdjustStockAsync(entity.WarehouseId, entity.ProductId, delta, entity.TransactionType);
        }

        private async Task<string?> AdjustStockAsync(int warehouseId, int productId, int delta, string transactionType)
        {
            var stock = await _stockRepo.GetAsync(warehouseId, productId);

            if (stock == null)
            {
                if (delta < 0)
                {
                    return transactionType == "Export"
                        ? "Insufficient stock to create export transaction."
                        : "Current stock is not enough to reverse this transaction.";
                }

                stock = new WarehouseStock
                {
                    WarehouseId = warehouseId,
                    ProductId = productId,
                    Quantity = delta,
                    UpdatedAt = DateTime.UtcNow
                };

                await _stockRepo.AddAsync(stock);
                return null;
            }

            var nextQuantity = stock.Quantity + delta;
            if (nextQuantity < 0)
            {
                return transactionType == "Export"
                    ? "Insufficient stock to create export transaction."
                    : "Current stock is not enough to reverse this transaction.";
            }

            stock.Quantity = nextQuantity;
            stock.UpdatedAt = DateTime.UtcNow;
            _stockRepo.Update(stock);
            return null;
        }

        private static bool DealerSupportsTransaction(string dealerType, string transactionType)
        {
            return dealerType == "Both" || dealerType == transactionType;
        }

        private static string? NormalizeTransactionType(string? value)
        {
            return (value ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "import" => "Import",
                "export" => "Export",
                _ => null
            };
        }

        private static InventoryTransactionDto ToDto(InventoryTransaction entity)
        {
            return new InventoryTransactionDto
            {
                Id = entity.Id,
                TransactionCode = entity.TransactionCode,
                TransactionType = entity.TransactionType,
                WarehouseId = entity.WarehouseId,
                WarehouseCode = entity.Warehouse?.WarehouseCode ?? string.Empty,
                WarehouseName = entity.Warehouse?.WarehouseName ?? string.Empty,
                ProductId = entity.ProductId,
                ProductCode = entity.Product?.ProductCode ?? string.Empty,
                ProductName = entity.Product?.ProductName ?? string.Empty,
                DealerId = entity.DealerId,
                DealerCode = entity.Dealer?.DealerCode ?? string.Empty,
                DealerName = entity.Dealer?.DealerName ?? string.Empty,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.UnitPrice * entity.Quantity,
                TransactionDate = entity.TransactionDate,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt
            };
        }

        private static IQueryable<InventoryTransaction> ApplySort(IQueryable<InventoryTransaction> query, string? sortBy, string? sortDir)
        {
            var desc = !string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
            return (sortBy ?? string.Empty).ToLowerInvariant() switch
            {
                "code" => desc ? query.OrderByDescending(t => t.TransactionCode) : query.OrderBy(t => t.TransactionCode),
                "type" => desc ? query.OrderByDescending(t => t.TransactionType) : query.OrderBy(t => t.TransactionType),
                "warehouse" => desc
                    ? query.OrderByDescending(t => t.Warehouse != null ? t.Warehouse.WarehouseName : string.Empty)
                    : query.OrderBy(t => t.Warehouse != null ? t.Warehouse.WarehouseName : string.Empty),
                "product" => desc
                    ? query.OrderByDescending(t => t.Product != null ? t.Product.ProductName : string.Empty)
                    : query.OrderBy(t => t.Product != null ? t.Product.ProductName : string.Empty),
                "dealer" => desc
                    ? query.OrderByDescending(t => t.Dealer != null ? t.Dealer.DealerName : string.Empty)
                    : query.OrderBy(t => t.Dealer != null ? t.Dealer.DealerName : string.Empty),
                "quantity" => desc ? query.OrderByDescending(t => t.Quantity) : query.OrderBy(t => t.Quantity),
                _ => desc ? query.OrderByDescending(t => t.TransactionDate) : query.OrderBy(t => t.TransactionDate)
            };
        }
    }
}
