using ConnectDB.Models;

namespace ConnectDB.Repositories
{
    public interface IStockRepository
    {
        IQueryable<WarehouseStock> Query();
        Task<WarehouseStock?> GetAsync(int warehouseId, int productId);
        Task AddAsync(WarehouseStock entity);
        void Update(WarehouseStock entity);
        void Remove(WarehouseStock entity);
        Task SaveChangesAsync();
    }
}
