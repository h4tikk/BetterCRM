using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context) { }

        private IQueryable<Ticket> BaseQuery => dbSet.Include(t => t.Assignee).Include(t => t.Creator);

        public async Task<List<Ticket>> GetForUsersAsync(Guid userId, string role, Guid? departmentId)
        {
            var q = BaseQuery;
            if(role == "DepartmentHead" && departmentId.HasValue)
            {
                var ids = await context.Users.Where(u => u.DepartmentId == departmentId.Value).Select(u => u.Id).ToListAsync();
                return await q.Where(t => ids.Contains(t.AssigneeId!.Value) || ids.Contains(t.CreatorId)).ToListAsync();
            }
            if(role == "Employee") return await q.Where(t => t.AssigneeId == userId || t.CreatorId == userId).ToListAsync();
            return await q.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<List<Ticket>> GetOverdueAsync() 
            => await BaseQuery.Where(t => t.IsSLABreached && (t.Status == "Open" || t.Status == "InProgress")).ToListAsync();
        public async Task<List<Ticket>> SearchAsync(string searchTerm) 
            => await BaseQuery.Where(t => t.Title.Contains(searchTerm) || (t.Description != null && t.Description.Contains(searchTerm))).ToListAsync();
        public async Task<Ticket?> GetByAssigneeAndIdAsync(Guid assigneeId, Guid id) 
            => await BaseQuery.FirstOrDefaultAsync(t => t.AssigneeId == assigneeId && t.Id == id);
        public async Task<Dictionary<string, int>> GetCountByStatusAsync(Guid? departmentId = null) 
        { 
            var q = dbSet.AsQueryable(); 
            if (departmentId.HasValue) 
            { 
                var ids = await context.Users.Where(u => u.DepartmentId == departmentId.Value).Select(u => u.Id).ToListAsync(); 
                q = q.Where(t => ids.Contains(t.AssigneeId!.Value) || ids.Contains(t.CreatorId)); 
            } 
            return await q.GroupBy(t => t.Status).ToDictionaryAsync(g => g.Key, g => g.Count()); 
        }
    }
}
