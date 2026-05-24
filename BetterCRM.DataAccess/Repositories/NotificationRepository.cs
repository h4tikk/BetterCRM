using BetterCRM.Core.Extensions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BetterCRM.DataAccess.Repositories
{
    public class NotificationRepository : Repository<Notification, NotificationEntity>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context) { }

        protected override Notification MapToDomain(NotificationEntity db) =>
            DomainMapper.ToNotificationDomain(db);

        protected override NotificationEntity MapToDb(Notification domain, NotificationEntity? existing = null) =>
            DomainMapper.ToNotificationDb(domain, existing);


        public async Task SaveManyAsync(List<Notification> notifications)
        {
            if (notifications.Count == 0) return;

            var entities = notifications.Select(n => MapToDb(n)).ToList();
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<Notification>> GetPagedAsync(
            Guid userId, int page, int pageSize)
        {
            var query = _dbSet
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            var total = await query.CountAsync();

            var items = (await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync())
                .Select(MapToDomain)
                .ToList();

            return new PagedResult<Notification>(items, total, page, pageSize);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId) =>
            await _dbSet
                .CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId) =>
            await _dbSet
                .Where(n => n.Id == notificationId && n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        public async Task MarkAllAsReadAsync(Guid userId) =>
            await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        public async Task<List<Guid>> GetTicketRecipientsAsync(
            Guid ticketId, Guid? assigneeId, Guid departmentId, Guid excludeUserId)
        {
            var recipients = new HashSet<Guid>();

            var creatorId = await _context.Tickets
                .Where(t => t.Id == ticketId)
                .Select(t => (Guid?)t.CreatorId)
                .FirstOrDefaultAsync();

            if (creatorId.HasValue)
                recipients.Add(creatorId.Value);

            if (assigneeId.HasValue)
                recipients.Add(assigneeId.Value);

            var heads = await _context.Users
                .Where(u => u.DepartmentId == departmentId &&
                            u.Role == "DepartmentHead" &&
                            u.IsActive)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var id in heads) recipients.Add(id);

            var orgId = await _context.Tickets
                .Where(t => t.Id == ticketId)
                .Select(t => t.OrganizationId)
                .FirstOrDefaultAsync();

            var admins = await _context.Users
                .Where(u => u.OrganizationId == orgId &&
                            u.Role == "Admin" &&
                            u.IsActive)
                .Select(u => u.Id)
                .ToListAsync();

            foreach (var id in admins) recipients.Add(id);

            recipients.Remove(excludeUserId);

            return recipients.ToList();
        }
    }
}
