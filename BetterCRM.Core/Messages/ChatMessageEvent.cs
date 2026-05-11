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
        DateTime SentAt
    );
}
