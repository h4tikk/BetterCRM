using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class TimeLogRepository : Repository<TimeLog, TimeLogEntity>, ITimeLogRepository
    {
        public TimeLogRepository(ApplicationDbContext context) : base(context) { }
        protected override TimeLog MapToDomain(TimeLogEntity db) => DomainMapper.ToTimeLogDomain(db);
        protected override TimeLogEntity MapToDb(TimeLog domain, TimeLogEntity? existing = null) => DomainMapper.ToTimeLogDb(domain, existing);

        public async Task<List<TimeLog>> GetByWorkSessionAsync(Guid sessionId)
        {
            var list = await _dbSet.Where(tl => tl.WorkSessionId == sessionId).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<List<TimeLog>> GetByTicketAsync(Guid ticketId)
        {
            var list = await _dbSet.Where(tl => tl.TicketId == ticketId).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<decimal> GetTotalHoursByTicketAsync(Guid ticketId) =>
            await _dbSet.Where(tl => tl.TicketId == ticketId).SumAsync(tl => tl.DurationHours);
    }
}
