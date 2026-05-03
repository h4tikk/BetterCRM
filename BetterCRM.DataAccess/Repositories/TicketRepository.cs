using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class TicketRepository : Repository<Ticket, TicketEntity>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context) { }
        protected override Ticket MapToDomain(TicketEntity db) => DomainMapper.ToTicketDomain(db);
        protected override TicketEntity MapToDb(Ticket domain, TicketEntity? existing = null) => DomainMapper.ToTicketDb(domain, existing);

        private IQueryable<TicketEntity> BaseQuery => _dbSet.Include(t => t.Assignee).Include(t => t.Creator);

        public async Task<List<Ticket>> GetForUsersAsync(Guid userId, string role, Guid? departmentId)
        {
            IQueryable<TicketEntity> q = BaseQuery;
            if (role == "DepartmentHead" && departmentId.HasValue)
            {
                var ids = await _context.Users.Where(u => u.DepartmentId == departmentId.Value).Select(u => u.Id).ToListAsync();
                q = q.Where(t => ids.Contains(t.AssigneeId!.Value) || ids.Contains(t.CreatorId));
            }
            else if (role == "Employee")
            {
                q = q.Where(t => t.AssigneeId == userId || t.CreatorId == userId);
            }
            var list = await q.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<List<Ticket>> GetOverdueAsync()
        {
            var list = await BaseQuery.Where(t => t.IsSLABreached && (t.Status == "Open" || t.Status == "InProgress")).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<List<Ticket>> SearchAsync(string searchTerm)
        {
            var list = await BaseQuery.Where(t => t.Title.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm))).ToListAsync();
            return list.Select(MapToDomain).ToList();
        }

        public async Task<Ticket?> GetByAssigneeAndIdAsync(Guid assigneeId, Guid id)
        {
            var db = await BaseQuery.FirstOrDefaultAsync(t => t.AssigneeId == assigneeId && t.Id == id);
            return db != null ? MapToDomain(db) : null;
        }

        public async Task<Dictionary<string, int>> GetCountByStatusAsync(Guid? departmentId = null)
        {
            IQueryable<TicketEntity> q = _dbSet;
            if (departmentId.HasValue)
            {
                var ids = await _context.Users.Where(u => u.DepartmentId == departmentId.Value).Select(u => u.Id).ToListAsync();
                q = q.Where(t => ids.Contains(t.AssigneeId!.Value) || ids.Contains(t.CreatorId));
            }
            return await q.GroupBy(t => t.Status).ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        
    }
}

