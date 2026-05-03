using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class WorkSessionRepository : Repository<WorkSession, WorkSessionEntity>, IWorkSessionRepository
    {
        public WorkSessionRepository(ApplicationDbContext context) : base(context) { }
        protected override WorkSession MapToDomain(WorkSessionEntity db) => DomainMapper.ToWorkSessionDomain(db);
        protected override WorkSessionEntity MapToDb(WorkSession domain, WorkSessionEntity? existing = null) => DomainMapper.ToWorkSessionDb(domain, existing);

        public async Task<WorkSession?> GetActiveSessionAsync(Guid userId)
        {
            var db = await _dbSet.FirstOrDefaultAsync(ws => ws.UserId == userId && ws.EndedAt == null);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<List<WorkSession>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<WorkSessionEntity> q = _dbSet.Where(ws => ws.UserId == userId && ws.EndedAt.HasValue);
            if (from.HasValue) q = q.Where(ws => ws.StartedAt >= from);
            if (to.HasValue) q = q.Where(ws => ws.StartedAt <= to);
            var list = await q.OrderByDescending(ws => ws.StartedAt).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<List<WorkSession>> GetByShiftAsync(Guid shiftId)
        {
            var list = await _dbSet.Where(ws => ws.ShiftId == shiftId && ws.EndedAt.HasValue).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<decimal> GetTotalHoursAsync(Guid userId, DateTime from, DateTime to) =>
            await _dbSet.Where(ws => ws.UserId == userId && ws.EndedAt.HasValue && ws.StartedAt >= from && ws.StartedAt <= to)
                        .SumAsync(ws => (decimal)(ws.EndedAt.Value - ws.StartedAt).TotalHours);

        public async Task<Dictionary<DateTime, decimal>> GetHoursByDayAsync(Guid userId, DateTime from, DateTime to)
        {
            var list = await _dbSet.Where(ws => ws.UserId == userId && ws.EndedAt.HasValue && ws.StartedAt >= from && ws.StartedAt <= to).ToListAsync();
            return list.GroupBy(ws => ws.StartedAt.Date)
                       .ToDictionary(g => g.Key, g => g.Sum(ws => (decimal)(ws.EndedAt!.Value - ws.StartedAt).TotalHours));
        }
    }
}
