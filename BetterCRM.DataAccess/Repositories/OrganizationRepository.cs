using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.DataAccess.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<OrganizationEntity> _dbSet;

        public OrganizationRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<OrganizationEntity>();
        }

        public async Task<Organization?> GetByIdAsync(Guid id)
        {
            var db = await _dbSet.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            return db != null ? DomainMapper.ToOrganizationDomain(db) : null;
        }

        public async Task<Organization?> GetBySlugAsync(string slug)
        {
            var db = await _dbSet.AsNoTracking().FirstOrDefaultAsync(o => o.Slug == slug);
            return db != null ? DomainMapper.ToOrganizationDomain(db) : null;
        }

        public async Task<Organization?> GetByIdWithDirectorAsync(Guid id)
        {
            var db = await _dbSet.Include(o => o.MainDirector).FirstOrDefaultAsync(o => o.Id == id);
            return db != null ? DomainMapper.ToOrganizationDomain(db) : null;
        }

        public async Task<Organization?> AddAsync(Organization entity)
        {
            var db = DomainMapper.ToOrganizationDb(entity);
            await _dbSet.AddAsync(db);
            await _context.SaveChangesAsync();
            return DomainMapper.ToOrganizationDomain(db);
        }

        public async Task UpdateAsync(Organization entity)
        {
            var db = await _dbSet.FindAsync(entity.Id);
            if (db == null) return;
            DomainMapper.ToOrganizationDb(entity, db);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Organization>> GetAllAsync()
        {
            var list = await _dbSet.AsNoTracking().ToListAsync();
            return list.Select(DomainMapper.ToOrganizationDomain).ToList();
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
            await _dbSet.AsNoTracking().AnyAsync(o => o.Id == id);
    }
}
