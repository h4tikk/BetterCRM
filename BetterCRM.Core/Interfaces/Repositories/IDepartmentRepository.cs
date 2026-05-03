using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department?> GetByNameAsync(string name);
        Task<bool> NameExistsAsync(string name);
        Task<List<Department>> GetWithUsersCountAsync();
    }
}
