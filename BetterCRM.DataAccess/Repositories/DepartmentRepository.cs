using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;


namespace BetterCRM.DataAccess.Repositories
{
    public class DepartmentRepository : Repository<Department, DepartmentEntity>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context) { }
        protected override Department MapToDomain(DepartmentEntity db) => DomainMapper.ToDepartmentDomain(db);
        protected override DepartmentEntity MapToDb(Department domain, DepartmentEntity? existing = null) => DomainMapper.ToDepartmentDb(domain, existing);

        public async Task<Department?> GetByNameAsync(string name)
        {
            name = name.Trim();
            var db = await _dbSet.AsNoTracking().FirstOrDefaultAsync(d => d.Name == name);
            return db is null ? null : MapToDomain(db);
        }

        public Task<bool> NameExistsAsync(string name)
        {
            name = name.Trim();

            return _dbSet.AsNoTracking().AnyAsync(d => d.Name == name);
        }

        public async Task<List<Department>> GetWithUsersCountAsync()
        {
            var list = await _dbSet.AsNoTracking().Include(d => d.Users).ToListAsync();

            return list.Select(MapToDomain).ToList();
        }
    }
}
