using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Messages;
using BetterCRM.Core.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BetterCRM.Business.Consumers
{
    public class ChatMessageConsumer : IConsumer<ChatMessageEvent>
    {
        private readonly IChatRepository _chatRepo;
        private readonly IChatNotifier _notifier;
        private readonly ILogger<ChatMessageConsumer> _logger;

        public ChatMessageConsumer(
            IChatRepository chatRepo,
            IChatNotifier notifier,
            ILogger<ChatMessageConsumer> logger)
        {
            _chatRepo = chatRepo;
            _notifier = notifier;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ChatMessageEvent> context)
        {
            var msg = context.Message;
            try
            {
                var (chatMessage, error) = ChatMessage.Create(
                    organizationId: msg.OrganizationId,
                    senderId: msg.SenderId,
                    text: msg.Text,
                    messageType: msg.MessageType,
                    attachmentUrl: msg.AttachmentUrl,
                    attachmentName: msg.AttachmentName,
                    attachmentObject: msg.AttachmentObject,
                    attachmentSize: msg.AttachmentSize,
                    attachmentMime: msg.AttachmentMime,
                    recipientId: msg.RecipientId,
                    chatRoomId: msg.ChatRoomId,
                    id: msg.MessageId,
                    sentAt: msg.SentAt
                );
                if(chatMessage is null)
                {
                    _logger.LogWarning("Невалидное сообщение: {Error}", error);
                    return;
                }
                await _chatRepo.SaveMessageAsync(chatMessage);

                var payload = new
                {
                    messageId = msg.MessageId,
                    senderId = msg.SenderId,
                    recipientId = msg.RecipientId,
                    chatRoomId = msg.ChatRoomId,
                    text = msg.Text,
                    messageType = msg.MessageType,
                    attachmentUrl = msg.AttachmentUrl,
                    attachmentName = msg.AttachmentName,
                    attachmentSize = msg.AttachmentSize,
                    attachmentMime = msg.AttachmentMime,
                    sentAt = msg.SentAt,
                    type = msg.RecipientId.HasValue ? "private" : "department"
                };

                if (msg.RecipientId.HasValue)
                    await _notifier.SendPrivateMessageAsync(payload, msg.RecipientId.Value);
                else if (msg.ChatRoomId.HasValue)
                    await _notifier.SendDepartmentMessageAsync(payload, msg.ChatRoomId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка ChatMessage {MessageId}", msg.MessageId);
                throw;
            }

        }
    }
}
