using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IOrganizationRepository
    {
        Task<Organization?> GetByIdAsync(Guid id);
        Task<Organization?> GetBySlugAsync(string slug);
        Task<Organization?> GetByIdWithDirectorAsync(Guid id);
        Task AddAsync(Organization organization);
        Task UpdateAsync(Organization organization);
    }
}
