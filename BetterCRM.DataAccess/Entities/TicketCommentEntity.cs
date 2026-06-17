namespace BetterCRM.DataAccess.Entities
{
    public class TicketCommentEntity : TenantDbEntity
    {
        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }
        public string Text { get; set; } = string.Empty;
        public TicketEntity Ticket { get; set; } = null!;
        public UserEntity Author { get; set; } = null!;
        public ICollection<TicketAttachmentEntity> Attachments { get; set; } = new List<TicketAttachmentEntity>();
    }
}
