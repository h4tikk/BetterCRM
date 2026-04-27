using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IWorkSessionRepository : IRepository<WorkSession>
    {
        Task<WorkSession?> GetActiveSessionAsync(Guid userId);
        Task<List<WorkSession>> GetAllActiveAsync();
        Task<List<WorkSession>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null);
        Task<List<WorkSession>> GetByUserAndTicketAsync(Guid userId, Guid ticketId);
        Task<int> GetTotalHoursAsync(Guid userId, DateTime from, DateTime to);
        Task<Dictionary<DateTime, int>> GetHoursByDayAsync(Guid userId, DateTime from, DateTime to);
        Task<List<WorkSession>> GetByTicketAsync(Guid ticketId);
        Task<int> GetTotalHoursByTicketAsync(Guid ticketId);

    }
}
