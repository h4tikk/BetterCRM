namespace BetterCRM.Core.Messages
{
    public record TicketNotificationEvent(
    Guid TicketId,
    Guid OrganizationId,
    string TicketTitle,
    NotifyType Type,
    Guid TriggeredByUserId,
    string TriggeredByName,
    Guid? AssigneeId,
    Guid DepartmentId,
    string? CommentText,
    DateTime OccurredAt,
    Guid? PreviousDepartmentId = null,
    Guid? PreviousAssigneeId = null
    );
}
