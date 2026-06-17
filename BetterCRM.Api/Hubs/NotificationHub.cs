using BetterCRM.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BetterCRM.Api.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationRepository _repo;

        public NotificationHub(INotificationRepository repo) => _repo = repo;

        private Guid CurrentUserId =>
            Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public override async Task OnConnectedAsync()
        {
            var unreadCount = await _repo.GetUnreadCountAsync(CurrentUserId);
            await Clients.Caller.SendAsync("UnreadCountUpdated", unreadCount);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception) =>
            base.OnDisconnectedAsync(exception);

        public async Task<object> GetNotifications(int page = 1, int pageSize = 20) =>
            await _repo.GetPagedAsync(CurrentUserId, page, pageSize);

        public async Task MarkAsRead(Guid notificationId)
        {
            await _repo.MarkAsReadAsync(notificationId, CurrentUserId);
            var unreadCount = await _repo.GetUnreadCountAsync(CurrentUserId);
            await Clients.Caller.SendAsync("UnreadCountUpdated", unreadCount);
        }

        public async Task MarkAllAsRead()
        {
            await _repo.MarkAllAsReadAsync(CurrentUserId);
            await Clients.Caller.SendAsync("UnreadCountUpdated", 0);
        }
    }
}
