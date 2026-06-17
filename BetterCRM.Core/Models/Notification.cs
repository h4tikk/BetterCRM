namespace BetterCRM.Core.Models
{
    public class Notification : TenantEntity
    {
        public Guid UserId { get; internal set; }
        public Guid? TicketId { get; internal set; }
        public string Type { get; internal set; } = string.Empty;
        public string Title { get; internal set; } = string.Empty;
        public string Body { get; internal set; } = string.Empty;
        public bool IsRead { get; internal set; }

        public const int MaxTitleLength = 300;
        public const int MaxBodyLength = 500;

        private Notification() { }

        public static (Notification? notification, string? error) Create(
            Guid organizationId,
            Guid userId,
            string type,
            string title,
            string body,
            Guid? ticketId = null)
        {
            if (userId == Guid.Empty)
                return (null, "Некорректный пользователь");

            if (string.IsNullOrWhiteSpace(title))
                return (null, "Заголовок уведомления не может быть пустым");

            if (title.Length > MaxTitleLength)
                return (null, $"Заголовок не может превышать {MaxTitleLength} символов");

            if (body.Length > MaxBodyLength)
                return (null, $"Текст не может превышать {MaxBodyLength} символов");

            return (new Notification
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = userId,
                TicketId = ticketId,
                Type = type,
                Title = title.Trim(),
                Body = body.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        public void MarkAsRead()
        {
            if (IsRead) return;
            IsRead = true;
            MarkAsUpdated();
        }
        internal static Notification Restore(
            Guid id, Guid organizationId, Guid userId,
            Guid? ticketId, string type, string title,
            string body, bool isRead, DateTime createdAt) =>
            new()
            {
                Id = id,
                OrganizationId = organizationId,
                UserId = userId,
                TicketId = ticketId,
                Type = type,
                Title = title,
                Body = body,
                IsRead = isRead,
                CreatedAt = createdAt
            };
    }
}
