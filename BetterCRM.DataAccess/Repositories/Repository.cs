using Microsoft.EntityFrameworkCore;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : TenantEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        public async Task<List<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();
        public async Task<T> AddAsync(T entity) 
        { 
            await _dbSet.AddAsync(entity); 
            await _context.SaveChangesAsync(); return entity; 
        }
        public async Task UpdateAsync(T entity) 
        { 
            _context.Entry(entity).State = EntityState.Modified; 
            await _context.SaveChangesAsync(); 
        }
        public async Task DeleteAsync(Guid id) 
        {
            var e = await _dbSet.FindAsync(id); 
            if (e != null) 
            { 
                _dbSet.Remove(e); 
                await _context.SaveChangesAsync(); 
            } 
        }
        public async Task<bool> ExistsAsync(Guid id) => await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id);

    }
}
