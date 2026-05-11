using BetterCRM.Api.Hubs;
using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.SignalR;

namespace BetterCRM.Api.Services
{
    public class ChatNotifier : IChatNotifier
    {
        private readonly IHubContext<ChatHub> _hub;
        public ChatNotifier(IHubContext<ChatHub> hub) => _hub = hub;

        public async Task SendDepartmentMessageAsync(object payload, Guid recipientId) =>
            await _hub.Clients.User(recipientId.ToString()).SendAsync("ReceiveMessage", payload);

        public async Task SendPrivateMessageAsync(object payload, Guid departmentId) =>
            await _hub.Clients.Group($"dept-{departmentId}").SendAsync("ReceiveMessage", payload);
    }
}
