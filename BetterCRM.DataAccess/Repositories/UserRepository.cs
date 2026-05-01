using Microsoft.EntityFrameworkCore;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email) => 
            await _dbSet.Include(u => u.Department).Include(u => u.Position).FirstOrDefaultAsync(u => u.Email == email);
        public async Task<List<User>> GetByDepartmentAsync(Guid departmentId) => 
            await _dbSet.Where(u => u.DepartmentId == departmentId).Include(u => u.Position).ToListAsync();
        public async Task<List<User>> GetActiveByDepartmentAsync(Guid departmentId) => 
            await _dbSet.Where(u => u.DepartmentId == departmentId && u.IsActive).ToListAsync();
        public async Task<bool> EmailExistsAsync(string email) =>
            await _dbSet.AnyAsync(u => u.Email == email);

    }
}
