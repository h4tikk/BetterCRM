using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T :BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
