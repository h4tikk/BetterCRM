using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Messages;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BetterCRM.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IPublishEndpoint _bus;
        private readonly IChatRepository _chatRepo;

        public ChatHub(IPublishEndpoint bus, IChatRepository chatRepo)
        {
            _bus = bus;
            _chatRepo = chatRepo;
        }

        private Guid CurrentUserId =>
            Guid.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private Guid CurrentOrganizationId =>
            Guid.Parse(Context.User!.FindFirstValue("organizationId")!);

        private static string DeptRoom(Guid departmentId) => $"dept-{departmentId}";


        public override async Task OnConnectedAsync()
        {
            var departmentId = await _chatRepo.GetUserDepartmentIdAsync(CurrentUserId);
            if (departmentId.HasValue)
                await Groups.AddToGroupAsync(Context.ConnectionId, DeptRoom(departmentId.Value));

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception) =>
            base.OnDisconnectedAsync(exception);

        public async Task SendPrivateMessage(Guid recipientId, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            await _bus.Publish(new ChatMessageEvent(
                MessageId: Guid.NewGuid(),
                OrganizationId: CurrentOrganizationId,
                SenderId: CurrentUserId,
                RecipientId: recipientId,
                ChatRoomId: null,
                Text: text.Trim(),
                SentAt: DateTime.UtcNow
            ));
        }

        public async Task JoinDepartmentRoom(Guid departmentId)
        {
            var belongs = await _chatRepo.UserBelongsToDepartmentAsync(CurrentUserId, departmentId);
            if (!belongs)
            {
                await Clients.Caller.SendAsync("Error", "Вы не состоите в этом отделе.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, DeptRoom(departmentId));
            await Clients.Caller.SendAsync("JoinedRoom", departmentId);
        }

        public async Task LeaveDepartmentRoom(Guid departmentId) =>
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, DeptRoom(departmentId));

        public async Task SendDepartmentMessage(Guid departmentId, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            var belongs = await _chatRepo.UserBelongsToDepartmentAsync(CurrentUserId, departmentId);
            if (!belongs)
            {
                await Clients.Caller.SendAsync("Error", "Нет доступа к этому отделу.");
                return;
            }

            await _bus.Publish(new ChatMessageEvent(
                MessageId: Guid.NewGuid(),
                OrganizationId: CurrentOrganizationId,
                SenderId: CurrentUserId,
                RecipientId: null,
                ChatRoomId: departmentId,
                Text: text.Trim(),
                SentAt: DateTime.UtcNow
            ));
        }

        public async Task<object> GetPrivateHistory(
            Guid withUserId, int take = 50, DateTime? before = null) =>
            await _chatRepo.GetPrivateMessagesAsync(
                CurrentUserId, withUserId, take, before ?? DateTime.UtcNow);

        public async Task<object> GetDepartmentHistory(
            Guid departmentId, int take = 50, DateTime? before = null)
        {
            var belongs = await _chatRepo.UserBelongsToDepartmentAsync(CurrentUserId, departmentId);
            if (!belongs) return Array.Empty<object>();

            return await _chatRepo.GetDepartmentMessagesAsync(
                departmentId, take, before ?? DateTime.UtcNow);
        }

        public async Task StartTyping(Guid recipientId) =>
            await Clients.User(recipientId.ToString())
                .SendAsync("UserTyping", CurrentUserId);

        public async Task StopTyping(Guid recipientId) =>
            await Clients.User(recipientId.ToString())
                .SendAsync("UserStoppedTyping", CurrentUserId);
    }
}
