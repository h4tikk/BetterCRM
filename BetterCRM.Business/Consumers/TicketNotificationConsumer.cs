using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Messages;
using BetterCRM.Core.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BetterCRM.Business.Consumers
{
    public class TicketNotificationConsumer : IConsumer<TicketNotificationEvent>
    {
        private readonly INotificationRepository _notifyRepo;
        private readonly ITicketNotifier _notifier;
        private readonly ILogger<TicketNotificationConsumer> _logger;

        public TicketNotificationConsumer(
            INotificationRepository notifyRepo,
            ITicketNotifier notifier,
            ILogger<TicketNotificationConsumer> logger)
        {
            _notifyRepo = notifyRepo;
            _notifier = notifier;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TicketNotificationEvent> context)
        {
            var e = context.Message;

            try
            {
                var recipients = await _notifyRepo.GetTicketRecipientsAsync(
                    e.TicketId, e.AssigneeId, e.DepartmentId, e.TriggeredByUserId);

                if (recipients.Count == 0) return;

                var notifications = new List<Notification>();

                foreach (var userId in recipients)
                {
                    var (notification, error) = Notification.Create(
                        organizationId: e.OrganizationId,
                        userId: userId,
                        type: e.Type.ToString(),
                        title: BuildTitle(e),
                        body: BuildBody(e),
                        ticketId: e.TicketId
                    );

                    if (notification is not null)
                        notifications.Add(notification);
                    else
                        _logger.LogWarning("Не удалось создать уведомление для {UserId}: {Error}", userId, error);
                }

                if (notifications.Count == 0) return;

                await _notifyRepo.SaveManyAsync(notifications);

                foreach (var n in notifications)
                {
                    await _notifier.SendNotificationAsync(new
                    {
                        id = n.Id,
                        title = n.Title,
                        body = n.Body,
                        type = n.Type,
                        ticketId = n.TicketId,
                        createdAt = n.CreatedAt,
                        isRead = false
                    }, n.UserId);

                    var unread = await _notifyRepo.GetUnreadCountAsync(n.UserId);
                    await _notifier.UpdateUnreadCountAsync(n.UserId, unread);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка TicketNotificationConsumer {TicketId}", e.TicketId);
                throw;
            }
        }

        private static string BuildTitle(TicketNotificationEvent e) => e.Type switch
        {
            NotifyType.TicketCreated => $"Новый тикет: {e.TicketTitle}",
            NotifyType.TicketApproved => $"Тикет подтверждён: {e.TicketTitle}",
            NotifyType.TicketRejected => $"Тикет отклонён: {e.TicketTitle}",
            NotifyType.TicketAssigned => $"Вам назначен тикет: {e.TicketTitle}",
            NotifyType.TicketResolved => $"Тикет решён: {e.TicketTitle}",
            NotifyType.TicketClosed => $"Тикет закрыт: {e.TicketTitle}",
            NotifyType.CommentAdded => $"Новый комментарий в тикете {e.TicketTitle} \n {e.CommentText}",
            NotifyType.AttachmentAdded => $"Новый файл: {e.TicketTitle}",
            _ => e.TicketTitle
        };

        private static string BuildBody(TicketNotificationEvent e) => e.Type switch
        {
            NotifyType.CommentAdded =>
               e.CommentText is { Length: > 0 } text
                    ? $"{e.TriggeredByName}: {text[..Math.Min(100, text.Length)]}"
                    : $"{e.TriggeredByName} оставил комментарий",

            NotifyType.TicketAssigned =>
                $"Назначил: {e.TriggeredByName}",

            _ =>
                $"Действие выполнил: {e.TriggeredByName}"
        };
    }
}
