using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class TimeLogRepository : Repository<TimeLog>, ITimeLogRepository
    {
        public TimeLogRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<TimeLog>> GetByWorkSessionAsync(Guid sessionId)
            => await dbSet.Where(tl => tl.WorkSessionId == sessionId).ToListAsync();

        public async Task<List<TimeLog>> GetByTicketAsync(Guid ticketId)
            => await dbSet.Where(tl => tl.TicketId == ticketId).ToListAsync();

        public async Task<decimal> GetTotalHoursByTicketAsync(Guid ticketId)
            => await dbSet.Where(tl => tl.TicketId == ticketId).SumAsync(tl => tl.DurationHours);
    }
}
