using ConnectDB.Models;

namespace ConnectDB.Repositories
{
    public interface IWarehouseRepository
    {
        IQueryable<Warehouse> Query();
        Task<Warehouse?> GetByIdAsync(int id);
        Task AddAsync(Warehouse entity);
        void Update(Warehouse entity);
        void Remove(Warehouse entity);
        Task<bool> ExistsCodeAsync(string code, int? excludeId = null);
        Task SaveChangesAsync();
    }
}
