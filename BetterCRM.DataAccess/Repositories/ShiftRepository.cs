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
            return (await q.OrderBy(s => s.Date).ToListAsync()).Select(MapToDomain).ToList();
        }

        // ✅ ИЗМЕНЕНО: было (Guid departmentId, DateTime date) — один день
        // Теперь принимает диапазон дат
        public async Task<List<Shift>> GetByDepartmentAsync(Guid departmentId, DateTime from, DateTime to)
        {
            var userIds = await _context.Users
                .Where(u => u.DepartmentId == departmentId)
                .Select(u => u.Id)
                .ToListAsync();

            return (await _dbSet
                .Where(s => userIds.Contains(s.UserId) && s.Date >= from && s.Date <= to)
                .OrderBy(s => s.Date)
                .ToListAsync()).Select(MapToDomain).ToList();
        }

        // ✅ НОВОЕ: расписание всей организации за период (для OrganizationHead / Admin)
        // Global query filter по OrganizationId уже применён в DbContext
        public async Task<List<Shift>> GetForOrganizationAsync(DateTime from, DateTime to)
        {
            return (await _dbSet
                .Include(s => s.User)
                .Where(s => s.Date >= from && s.Date <= to)
                .OrderBy(s => s.Date).ThenBy(s => s.User.DepartmentId)
                .ToListAsync()).Select(MapToDomain).ToList();
        }

        public async Task<Shift?> GetByUserAndDateAsync(Guid userId, DateTime date)
        {
            var db = await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId && s.Date == date.Date);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<decimal> GetTotalScheduledHoursAsync(Guid userId, DateTime from, DateTime to) =>
            await _dbSet
                .Where(s => s.UserId == userId && s.Date >= from && s.Date <= to)
                .SumAsync(s => (decimal)(s.EndTime - s.StartTime).TotalHours);


        public async Task<decimal> GetTotalAttendancePenaltyAsync(Guid userId, DateTime from, DateTime to) =>
            await _dbSet
                .Where(s => s.UserId == userId && s.Date >= from && s.Date <= to)
                .SumAsync(s => s.LatenessPenaltyHours + s.EarlyLeavePenaltyHours);
    }
}