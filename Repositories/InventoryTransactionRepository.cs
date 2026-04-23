using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Repositories
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly AppDbContext _context;

        public InventoryTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<InventoryTransaction> Query()
        {
            return _context.InventoryTransactions
                .Include(t => t.Warehouse)
                .Include(t => t.Product)
                .ThenInclude(p => p!.Category)
                .Include(t => t.Dealer)
                .AsQueryable();
        }

        public Task<InventoryTransaction?> GetByIdAsync(int id)
        {
            return _context.InventoryTransactions
                .Include(t => t.Warehouse)
                .Include(t => t.Product)
                .ThenInclude(p => p!.Category)
                .Include(t => t.Dealer)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public Task AddAsync(InventoryTransaction entity)
        {
            return _context.InventoryTransactions.AddAsync(entity).AsTask();
        }

        public void Update(InventoryTransaction entity)
        {
            _context.InventoryTransactions.Update(entity);
        }

        public void Remove(InventoryTransaction entity)
        {
            _context.InventoryTransactions.Remove(entity);
        }

        public Task<bool> ExistsCodeAsync(string code, int? excludeId = null)
        {
            var query = _context.InventoryTransactions.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return query.AnyAsync(t => t.TransactionCode == code);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
