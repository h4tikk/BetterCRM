using BetterCRM.Api.Hubs;
using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace BetterCRM.Api.Services
{
    public class TicketNotifier : ITicketNotifier
    {
        private readonly IHubContext<NotificationHub> _hub;

        public TicketNotifier(IHubContext<NotificationHub> hub) => _hub = hub;

        public async Task SendNotificationAsync(object payload, Guid recipientId) =>
            await _hub.Clients
                .User(recipientId.ToString())
                .SendAsync("ReceiveNotification", payload);

        public async Task UpdateUnreadCountAsync(Guid userId, int count) =>
            await _hub.Clients
                .User(userId.ToString())
                .SendAsync("UnreadCountUpdated", count);
    }
}
