namespace BetterCRM.Core.Messages
{
    public record ChatMessageEvent
    (
        Guid OrganizationId,
        Guid MessageId,
        Guid SenderId,
        Guid? RecipientId,
        Guid? ChatRoomId,
        string Text,
        DateTime SentAt,
        string MessageType = "text",
        string? AttachmentObject = null,
        string? AttachmentUrl = null,
        string? AttachmentName = null,
        long? AttachmentSize = null,
        string? AttachmentMime = null
    );
}
