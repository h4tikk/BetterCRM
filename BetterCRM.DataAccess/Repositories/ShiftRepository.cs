using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class ShiftRepository : Repository<Shift, ShiftEntity>, IShiftRepository
    {
        public ShiftRepository(ApplicationDbContext context) : base(context) { }
        protected override Shift MapToDomain(ShiftEntity db) => DomainMapper.ToShiftDomain(db);
        protected override ShiftEntity MapToDb(Shift domain, ShiftEntity? existing = null) => DomainMapper.ToShiftDb(domain, existing);

        public async Task<List<Shift>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null)
        {
            IQueryable<ShiftEntity> q = _dbSet.Where(s => s.UserId == userId);
            if (from.HasValue) q = q.Where(s => s.Date >= from.Value);
            if (to.HasValue) q = q.Where(s => s.Date <= to.Value);
            var list = await q.OrderBy(s => s.Date).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<List<Shift>> GetByDepartmentAsync(Guid departmentId, DateTime date)
        {
            var ids = await _context.Users.Where(u => u.DepartmentId == departmentId).Select(u => u.Id).ToListAsync();
            var list = await _dbSet.Where(s => ids.Contains(s.UserId) && s.Date == date).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<Shift?> GetByUserAndDateAsync(Guid userId, DateTime date)
        {
            var db = await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId && s.Date == date);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<decimal> GetTotalScheduledHoursAsync(Guid userId, DateTime from, DateTime to) =>
            await _dbSet.Where(s => s.UserId == userId && s.Date >= from && s.Date <= to)
                        .SumAsync(s => (decimal)(s.EndTime - s.StartTime).TotalHours);
    }
}
