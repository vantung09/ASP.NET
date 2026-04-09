using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> Query()
        {
            return _context.Products.Include(p => p.Category).AsQueryable();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            return _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task AddAsync(Product entity)
        {
            return _context.Products.AddAsync(entity).AsTask();
        }

        public void Update(Product entity)
        {
            _context.Products.Update(entity);
        }

        public void Remove(Product entity)
        {
            _context.Products.Remove(entity);
        }

        public Task<bool> ExistsCodeAsync(string code, int? excludeId = null)
        {
            var query = _context.Products.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return query.AnyAsync(p => p.ProductCode == code);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
