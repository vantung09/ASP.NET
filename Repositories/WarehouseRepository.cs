using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AppDbContext _context;

        public WarehouseRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Warehouse> Query()
        {
            return _context.Warehouses.AsQueryable();
        }

        public Task<Warehouse?> GetByIdAsync(int id)
        {
            return _context.Warehouses.FirstOrDefaultAsync(w => w.Id == id);
        }

        public Task AddAsync(Warehouse entity)
        {
            return _context.Warehouses.AddAsync(entity).AsTask();
        }

        public void Update(Warehouse entity)
        {
            _context.Warehouses.Update(entity);
        }

        public void Remove(Warehouse entity)
        {
            _context.Warehouses.Remove(entity);
        }

        public Task<bool> ExistsCodeAsync(string code, int? excludeId = null)
        {
            var query = _context.Warehouses.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(w => w.Id != excludeId.Value);
            }
            return query.AnyAsync(w => w.WarehouseCode == code);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
