using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface ITimeLogRepository : IRepository<TimeLog>
    {
        Task<List<TimeLog>> GetByWorkSessionAsync(Guid sessionId);
        Task<List<TimeLog>> GetByTicketAsync(Guid ticketId);
        Task<decimal> GetTotalHoursByTicketAsync(Guid ticketId);
    }
}
