using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;

        public StockRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<WarehouseStock> Query()
        {
            return _context.WarehouseStocks
                .Include(ws => ws.Warehouse)
                .Include(ws => ws.Product)
                .ThenInclude(p => p!.Category)
                .AsQueryable();
        }

        public Task<WarehouseStock?> GetAsync(int warehouseId, int productId)
        {
            return _context.WarehouseStocks
                .Include(ws => ws.Warehouse)
                .Include(ws => ws.Product)
                .ThenInclude(p => p!.Category)
                .FirstOrDefaultAsync(ws => ws.WarehouseId == warehouseId && ws.ProductId == productId);
        }

        public Task AddAsync(WarehouseStock entity)
        {
            return _context.WarehouseStocks.AddAsync(entity).AsTask();
        }

        public void Update(WarehouseStock entity)
        {
            _context.WarehouseStocks.Update(entity);
        }

        public void Remove(WarehouseStock entity)
        {
            _context.WarehouseStocks.Remove(entity);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
