using BetterCRM.Core.Extensions;
using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task SaveManyAsync(List<Notification> notifications);
        Task<PagedResult<Notification>> GetPagedAsync(Guid userId, int page, int pageSize);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
        Task<List<Guid>> GetTicketRecipientsAsync(
            Guid ticketId,
            Guid? assigneeId,
            Guid departmentId,
            Guid excludeUserId);
    }
}
