using ConnectDB.Models;

namespace ConnectDB.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> Query();
        Task<Category?> GetByIdAsync(int id);
        Task AddAsync(Category entity);
        void Update(Category entity);
        void Remove(Category entity);
        Task<bool> ExistsCodeAsync(string code, int? excludeId = null);
        Task SaveChangesAsync();
    }
}
