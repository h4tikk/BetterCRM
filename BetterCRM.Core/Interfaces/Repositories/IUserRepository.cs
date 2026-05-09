using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetByDepartmentAsync(Guid departmentId);
        Task<List<User>> GetActiveByDepartmentAsync(Guid departmentId);
        Task<bool> EmailExistsAsync(string email);
    }
}
