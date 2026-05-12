namespace BetterCRM.Core.Models
{
    public class TicketAttachment : TenantEntity
    {
        public Guid TicketId { get; internal set; }
        public Guid? CommentId { get; internal set; }
        public Guid UploaderId { get; internal set; }
        public string FileName { get; internal set; } = string.Empty;
        public string ObjectName { get; internal set; } = string.Empty;
        public string ContentType { get; internal set; } = string.Empty;
        public long SizeBytes { get; internal set; }
        public string? Url { get; internal set; }

        private TicketAttachment() { }

        public static TicketAttachment Restore(
            Guid id, Guid organizationId, Guid ticketId,
            Guid? commentId, Guid uploaderId, string fileName,
            string objectName, string contentType,
            long sizeBytes, DateTime createdAt, string? url = null) =>
            new()
            {
                Id = id,
                OrganizationId = organizationId,
                TicketId = ticketId,
                CommentId = commentId,
                UploaderId = uploaderId,
                FileName = fileName,
                ObjectName = objectName,
                ContentType = contentType,
                SizeBytes = sizeBytes,
                CreatedAt = createdAt,
                Url = url
            };

    }
}
