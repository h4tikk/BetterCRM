namespace BetterCRM.Core.Models
{
    public class TicketComment : TenantEntity
    {
        public Guid TicketId { get; internal set; }
        public Guid AuthorId { get; internal set; }
        public string AuthorName { get; internal set; } = string.Empty;
        public string Text { get; internal set; } = string.Empty;
        public IReadOnlyList<TicketAttachment> Attachments { get; internal set; } = [];

        public const int MaxTextLength = 4000;

        private TicketComment() { }

        public static (TicketComment? comment, string? error) Create(
            Guid organizationId, Guid ticketId, Guid authorId, string authorName, string text)
        {
            text = text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text))
                return (null, "Комментарий не может быть пустым");

            if (text.Length > MaxTextLength)
                return (null, $"Комментарий не может превышать {MaxTextLength} символов");

            if (ticketId == Guid.Empty)
                return (null, "Некорректный тикет");

            if (authorId == Guid.Empty)
                return (null, "Некорректный автор");

            return (new TicketComment
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                TicketId = ticketId,
                AuthorId = authorId,
                AuthorName = authorName,
                Text = text,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        internal static TicketComment Restore(
            Guid id, Guid organizationId, Guid ticketId,
            Guid authorId, string authorName, string text,
            DateTime createdAt, DateTime? updatedAt,
            List<TicketAttachment> attachments) =>
            new()
            {
                Id = id,
                OrganizationId = organizationId,
                TicketId = ticketId,
                AuthorId = authorId,
                AuthorName = authorName,
                Text = text,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                Attachments = attachments
            };

        public void Update(string newText)
        {
            if (string.IsNullOrWhiteSpace(newText))
                throw new InvalidOperationException("Комментарий не может быть пустым");
            Text = newText.Trim();
            UpdatedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }
    }
}
