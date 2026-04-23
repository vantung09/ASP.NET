using ConnectDB.Models;

namespace ConnectDB.Repositories
{
    public interface IInventoryTransactionRepository
    {
        IQueryable<InventoryTransaction> Query();
        Task<InventoryTransaction?> GetByIdAsync(int id);
        Task AddAsync(InventoryTransaction entity);
        void Update(InventoryTransaction entity);
        void Remove(InventoryTransaction entity);
        Task<bool> ExistsCodeAsync(string code, int? excludeId = null);
        Task SaveChangesAsync();
    }
}
