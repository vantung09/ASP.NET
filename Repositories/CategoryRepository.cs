using ConnectDB.Data;
using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> Query()
        {
            return _context.Categories.AsQueryable();
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            return _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task AddAsync(Category entity)
        {
            return _context.Categories.AddAsync(entity).AsTask();
        }

        public void Update(Category entity)
        {
            _context.Categories.Update(entity);
        }

        public void Remove(Category entity)
        {
            _context.Categories.Remove(entity);
        }

        public Task<bool> ExistsCodeAsync(string code, int? excludeId = null)
        {
            var query = _context.Categories.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return query.AnyAsync(c => c.CategoryCode == code);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
