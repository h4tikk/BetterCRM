using Microsoft.EntityFrameworkCore;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Repositories
{
    public class ShiftRepository : Repository<Shift>, IShiftRepository
    {
        public ShiftRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<Shift>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null)
        {
            var q = dbSet.Where(s => s.UserId == userId); 
            if (from.HasValue) 
                q = q.Where(s => s.Date >= from.Value); 
            if (to.HasValue) 
                q = q.Where(s => s.Date <= to.Value); 
            return await q.OrderBy(s => s.Date).ToListAsync();
        }
        public async Task<List<Shift>> GetByDepartmentAsync(Guid departmentId, DateTime date) 
        { 
            var ids = await context.Users.Where(u => u.DepartmentId == departmentId).Select(u => u.Id).ToListAsync(); 
            return await dbSet.Where(s => ids.Contains(s.UserId) && s.Date == date).ToListAsync(); 
        }
        public async Task<Shift?> GetByUserAndDateAsync(Guid userId, DateTime date) 
            => await dbSet.FirstOrDefaultAsync(s => s.UserId == userId && s.Date == date);
        public async Task<decimal> GetTotalScheduledHoursAsync(Guid userId, DateTime from, DateTime to) 
            => await dbSet.Where(s => s.UserId == userId && s.Date >= from && s.Date <= to).SumAsync(s => (decimal)(s.EndTime - s.StartTime).TotalHours);
    }
}
