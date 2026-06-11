namespace BetterCRM.DataAccess.Entities
{
    public class ChatMessageEntity : TenantDbEntity
    {
        public Guid SenderId { get; set; }
        public UserEntity Sender { get; set; } = null!;

        public Guid? RecipientId { get; set; }
        public UserEntity? Recipient { get; set; }

        public Guid? ChatRoomId { get; set; }

        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }

        public string MessageType { get; set; } = "text";
        public string? AttachmentObjectName { get; set; }
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public string? AttachmentMime { get; set; }
    }
}
