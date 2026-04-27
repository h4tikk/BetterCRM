using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<List<Ticket>> GetByCreatorAsync(Guid creatorId);
        Task<List<Ticket>> GetByAssigneeAsync(Guid assigneeId);
        Task<List<Ticket>> GetByParticipantAsync(Guid userId);
        Task<List<Ticket>> GetByDepartmentAsync(Guid departmentId);
        Task<List<Ticket>> GetForUserAsync(Guid userId, string role, Guid? departmentId);
        Task<List<Ticket>> GetByStatusAsync(string status);
        Task<List<Ticket>> GetOverdueAsync(); // Нарушенные SLA
        Task<List<Ticket>> SearchAsync(string searchTerm, Guid? departmentId = null);
        Task<Dictionary<string, int>> GetTicketsCountByStatusAsync(Guid? departmentId = null);
    }
}
