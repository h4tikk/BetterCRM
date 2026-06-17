using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IOrganizationRepository
    {
        Task<List<Organization>> GetAllAsync();
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Organization?> GetByIdAsync(Guid id);
        Task<Organization?> GetBySlugAsync(string slug);
        Task<Organization?> GetByIdWithDirectorAsync(Guid id);
        Task<Organization?> AddAsync(Organization organization);
        Task UpdateAsync(Organization organization);
    }
}
