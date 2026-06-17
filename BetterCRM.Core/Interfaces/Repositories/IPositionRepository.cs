using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IPositionRepository : IRepository<Position>
    {
        Task<Position?> GetByTitleAsync(string title);
        Task<List<Position>> GetByDepartmentAsync(Guid departmentId);
        Task<bool> TitleExistsInDepartmentAsync(string title, Guid departmentId);
    }
}
