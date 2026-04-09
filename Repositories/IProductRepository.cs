using ConnectDB.Models;

namespace ConnectDB.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> Query();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product entity);
        void Update(Product entity);
        void Remove(Product entity);
        Task<bool> ExistsCodeAsync(string code, int? excludeId = null);
        Task SaveChangesAsync();
    }
}
