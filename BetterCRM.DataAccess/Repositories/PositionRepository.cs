using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;


namespace BetterCRM.DataAccess.Repositories
{
    public class PositionRepository : Repository<Position, PositionEntity>, IPositionRepository
    {
        public PositionRepository(ApplicationDbContext context) : base(context) { }
        protected override Position MapToDomain(PositionEntity db) => DomainMapper.ToPositionDomain(db);
        protected override PositionEntity MapToDb(Position domain, PositionEntity? existing = null) => DomainMapper.ToPositionDb(domain, existing);

        public async Task<Position?> GetByTitleAsync(string title)
        {
            title = title.Trim();
            var db = await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Title == title);
            return db != null ? MapToDomain(db) : null; 
        }

        public async Task<List<Position>> GetByDepartmentAsync(Guid departmentId)
        {
            var list = await _dbSet
                .AsNoTracking()
                .Include(p => p.Users)
                .Where(p => p.Users.Any(u => u.DepartmentId == departmentId))
                .ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<bool> TitleExistsInDepartmentAsync(string title, Guid departmentId)
        {
            title = title.Trim();

            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Users)
                .AnyAsync(p => p.Title == title && p.DepartmentId == departmentId);
        }
    }
}
