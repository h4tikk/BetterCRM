using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class UserRepository : Repository<User, UserEntity>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }
        protected override User MapToDomain(UserEntity db) => DomainMapper.ToUserDomain(db);
        protected override UserEntity MapToDb(User domain, UserEntity? existing = null) => DomainMapper.ToUserDb(domain, existing);

        public async Task<User?> GetByEmailAsync(string email)
        {
            var db = await _dbSet.Include(u => u.Department).Include(u => u.Position)
                                 .FirstOrDefaultAsync(u => u.Email == email);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<List<User>> GetByDepartmentAsync(Guid departmentId)
        {
            var list = await _dbSet.Where(u => u.DepartmentId == departmentId)
                                   .Include(u => u.Position)
                                   .ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<List<User>> GetActiveByDepartmentAsync(Guid departmentId)
        {
            var list = await _dbSet.Where(u => u.DepartmentId == departmentId && u.IsActive)
                                   .ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<bool> EmailExistsAsync(string email) =>
            await _dbSet.AsNoTracking().AnyAsync(u => u.Email == email);
    }
}
