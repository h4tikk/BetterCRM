namespace BetterCRM.DataAccess.Entities
{
    public class TicketAttachmentEntity : TenantDbEntity
    {
        public Guid TicketId { get; set; }
        public Guid? CommentId { get; set; }
        public Guid UploaderId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public TicketEntity Ticket { get; set; } = null!;
        public TicketCommentEntity? Comment { get; set; }
        public UserEntity Uploader { get; set; } = null!;
    }
}
