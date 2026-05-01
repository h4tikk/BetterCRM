using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class WorkSessionRepository : Repository<WorkSession>, IWorkSessionRepository
    {
        public WorkSessionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<WorkSession?> GetActiveSessionAsync(Guid userId) =>
            await _dbSet.FirstOrDefaultAsync(ws => ws.UserId == userId && ws.EndedAt == null);
        public async Task<List<WorkSession>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null)
        {
            var q = _dbSet.Where(ws => ws.UserId == userId && ws.EndedAt.HasValue);
            if (from.HasValue) 
                q = q.Where(ws => ws.StartedAt >= from);
            if (to.HasValue) 
                q = q.Where(ws => ws.StartedAt <= to); 
            return await q.OrderByDescending(ws => ws.StartedAt).ToListAsync();
        }

        public async Task<decimal> GetTotalHoursAsync(Guid userId, DateTime from, DateTime to) => 
            await _dbSet.Where(ws => ws.UserId == userId && ws.EndedAt.HasValue && ws.StartedAt >= from && ws.StartedAt <= to).SumAsync(ws => (decimal)(ws.EndedAt!.Value - ws.StartedAt).TotalHours);
        public async Task<Dictionary<DateTime, decimal>> GetHoursByDayAsync(Guid userId, DateTime from, DateTime to) => 
            await _dbSet.Where(ws => ws.UserId == userId && ws.EndedAt.HasValue && ws.StartedAt >= from && ws.StartedAt <= to).GroupBy(ws => ws.StartedAt.Date).ToDictionaryAsync(g => g.Key, g => g.Sum(ws => (decimal)(ws.EndedAt!.Value - ws.StartedAt).TotalHours));
        public async Task<List<WorkSession>> GetByShiftAsync(Guid shiftId) =>  
            await _dbSet.Where(ws => ws.ShiftId == shiftId && ws.EndedAt.HasValue).ToListAsync();
    }
}
