using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class ShiftBreakRepository : Repository<ShiftBreak, ShiftBreakEntity>, IShiftBreakRepository
    {
        public ShiftBreakRepository(ApplicationDbContext context) : base(context) { }

        protected override ShiftBreak MapToDomain(ShiftBreakEntity db) => DomainMapper.ToShiftBreakDomain(db);
        protected override ShiftBreakEntity MapToDb(ShiftBreak domain, ShiftBreakEntity? existing = null) => DomainMapper.ToShiftBreakDb(domain, existing);

        public async Task<List<ShiftBreak>> GetByShiftAsync(Guid shiftId)
        {
            var list = await _dbSet.Where(b => b.ShiftId == shiftId).OrderBy(b => b.StartTime).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }
    }
}
