using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public abstract class Repository<TDomain, TDb> : IRepository<TDomain>
    where TDomain : TenantEntity
    where TDb : TenantDbEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TDb> _dbSet;

        protected Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TDb>();
        }

        protected abstract TDomain MapToDomain(TDb db);
        protected abstract TDb MapToDb(TDomain domain, TDb? existing = null);

        public async Task<TDomain?> GetByIdAsync(Guid id)
        {
            var db = await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<List<TDomain>> GetAllAsync()
        {
            var list = await _dbSet.AsNoTracking().ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<TDomain> AddAsync(TDomain domain)
        {
            var db = MapToDb(domain);
            await _dbSet.AddAsync(db);
            await _context.SaveChangesAsync();
            return MapToDomain(db);
        }

        public async Task UpdateAsync(TDomain domain)
        {
            var db = await _dbSet.FindAsync(domain.Id);
            if (db == null) return;
            MapToDb(domain, db);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var db = await _dbSet.FindAsync(id);
            if (db != null)
            {
                _dbSet.Remove(db);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id) =>
            await _dbSet.AsNoTracking().AnyAsync(e => e.Id == id);
    }
}
