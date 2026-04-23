using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Repositories
{
    public class DealerRepository : IDealerRepository
    {
        private readonly AppDbContext _context;

        public DealerRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Dealer> Query()
        {
            return _context.Dealers.AsQueryable();
        }

        public Task<Dealer?> GetByIdAsync(int id)
        {
            return _context.Dealers.FirstOrDefaultAsync(d => d.Id == id);
        }

        public Task AddAsync(Dealer entity)
        {
            return _context.Dealers.AddAsync(entity).AsTask();
        }

        public void Update(Dealer entity)
        {
            _context.Dealers.Update(entity);
        }

        public void Remove(Dealer entity)
        {
            _context.Dealers.Remove(entity);
        }

        public Task<bool> ExistsCodeAsync(string code, int? excludeId = null)
        {
            var query = _context.Dealers.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return query.AnyAsync(d => d.DealerCode == code);
        }

        public Task<bool> HasTransactionsAsync(int dealerId)
        {
            return _context.InventoryTransactions.AnyAsync(t => t.DealerId == dealerId);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
