namespace BetterCRM.Core.Models
{
    public class ChatMessage : TenantEntity
    {
        public Guid SenderId { get; internal set; }
        public Guid? RecipientId { get; internal set; }
        public Guid? ChatRoomId { get; internal set; }
        public string Text { get; internal set; } = string.Empty;
        public DateTime SentAt { get; internal set; }
        public bool IsRead { get; internal set; }
        public string SenderName { get; internal set; } = string.Empty;
        public string? SenderAvatar { get; internal set; }
        public string? RecipientName { get; internal set; }

        public const int MaxTextLength = 2000;
        public const int MinTextLength = 1;

        protected ChatMessage() { }

        public static (ChatMessage? message, string? error) Create(
            Guid organizationId,
            Guid senderId,
            string text,
            Guid? recipientId = null,
            Guid? chatRoomId = null)
        {
            text = text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text))
                return (null, "Сообщение не может быть пустым");

            if (text.Length > MaxTextLength)
                return (null, $"Сообщение не может превышать {MaxTextLength} символов");

            if (senderId == Guid.Empty)
                return (null, "Некорректный отправитель");

            if (recipientId == null && chatRoomId == null)
                return (null, "Необходимо указать получателя или комнату");

            if (recipientId.HasValue && chatRoomId.HasValue)
                return (null, "Нельзя одновременно указать получателя и комнату");

            if (recipientId.HasValue && recipientId == senderId)
                return (null, "Нельзя отправить сообщение самому себе");

            return (new ChatMessage
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                SenderId = senderId,
                RecipientId = recipientId,
                ChatRoomId = chatRoomId,
                Text = text,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        internal static ChatMessage Restore(
            Guid id, Guid organizationId,
            Guid senderId, string senderName, string? senderAvatar,
            Guid? recipientId, string? recipientName,
            Guid? chatRoomId, string text,
            DateTime sentAt, bool isRead, DateTime createdAt) =>
            new()
            {
                Id = id,
                OrganizationId = organizationId,
                SenderId = senderId,
                SenderName = senderName,
                SenderAvatar = senderAvatar,
                RecipientId = recipientId,
                RecipientName = recipientName,
                ChatRoomId = chatRoomId,
                Text = text,
                SentAt = sentAt,
                IsRead = isRead,
                CreatedAt = createdAt
            };

        public void MarkAsRead()
        {
            if (IsRead) return;
            IsRead = true;
            MarkAsUpdated();
        }

        public bool IsPrivate => RecipientId.HasValue;
        public bool IsDepartment => ChatRoomId.HasValue;
    }
}
