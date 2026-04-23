using ConnectDB.Models;

namespace ConnectDB.Repositories
{
    public interface IDealerRepository
    {
        IQueryable<Dealer> Query();
        Task<Dealer?> GetByIdAsync(int id);
        Task AddAsync(Dealer entity);
        void Update(Dealer entity);
        void Remove(Dealer entity);
        Task<bool> ExistsCodeAsync(string code, int? excludeId = null);
        Task<bool> HasTransactionsAsync(int dealerId);
        Task SaveChangesAsync();
    }
}
