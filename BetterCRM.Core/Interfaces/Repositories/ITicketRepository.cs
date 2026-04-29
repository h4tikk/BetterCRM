using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<List<Ticket>> GetByCreatorAsync(Guid creatorId);
        Task<List<Ticket>> GetByAssigneeAndIdAsync(Guid assigneeId, Guid id);
        Task<List<Ticket>> GetByDepartmentAsync(Guid departmentId);
        Task<List<Ticket>> GetForUsersAsync();
        Task<List<Ticket>> GetOverdueAsync(); 
        Task<List<Ticket>> SearchAsync(string searchTerm, Guid? departmentId = null);
        Task<Dictionary<string, int>> GetTicketsCountByStatusAsync(Guid? departmentId = null);
    }
}
