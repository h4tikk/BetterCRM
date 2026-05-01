using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<Ticket?> GetByAssigneeAndIdAsync(Guid assigneeId, Guid id);
        Task<List<Ticket>> GetForUsersAsync(Guid userId, string role, Guid? departmentId);
        Task<List<Ticket>> GetOverdueAsync(); 
        Task<List<Ticket>> SearchAsync(string searchTerm);
        Task<Dictionary<string, int>> GetCountByStatusAsync(Guid? departmentId = null);
    }
}
