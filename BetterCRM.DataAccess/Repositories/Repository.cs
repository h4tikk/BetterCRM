using Microsoft.EntityFrameworkCore;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : TenantEntity
    {
        protected readonly ApplicationDbContext context;
        protected readonly DbSet<T> dbSet;

        public Repository(ApplicationDbContext _context)
        {
            context = _context;
            dbSet = _context.Set<T>();
        }
        public async Task<T?> GetByIdAsync(Guid id) => await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        public async Task<List<T>> GetAllAsync() => await dbSet.AsNoTracking().ToListAsync();
        public async Task<T> AddAsync(T entity) { await dbSet.AddAsync(entity); await context.SaveChangesAsync(); return entity; }
        public async Task UpdateAsync(T entity) { context.Entry(entity).State = EntityState.Modified; await context.SaveChangesAsync(); }
        public async Task DeleteAsync(Guid id) { var e = await dbSet.FindAsync(id); if (e != null) { dbSet.Remove(e); await context.SaveChangesAsync(); } }
        public async Task<bool> ExistsAsync(Guid id) => await dbSet.AsNoTracking().AnyAsync(e => e.Id == id);

    }
}
