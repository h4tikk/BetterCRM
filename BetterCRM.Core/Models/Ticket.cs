namespace BetterCRM.Core.Models
{
    public class Ticket : TenantEntity
    {
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string Priority { get; private set; } = string.Empty;
        public string Status { get; private set; } = string.Empty;
        public Guid CreatorId { get; private set; }
        public Guid? AssigneeId { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public decimal SLATargetHours { get; private set; }
        public bool IsSLABreached { get; private set; }

        public User Creator { get; private set; } = null!;
        public User? Assignee { get; private set; }
        public ICollection<TicketParticipant> Participants { get; private set; } = new List<TicketParticipant>();
        public ICollection<TimeLog> TimeLogs { get; private set; } = new List<TimeLog>();

        public const int MinTitleLength = 1;
        public const int MaxTitleLength = 200;
        public const int MaxDescriptionLength = 1000;
        public static readonly string[] ValidPriorities = { "High", "Medium", "Low" };
        public static readonly string[] ValidStatuses = { "Open", "InProgress", "Resolved", "Closed" };
        public static readonly Dictionary<string, decimal> DefaultSLATargets = new() { { "High", 4 }, { "Medium", 8 }, { "Low", 24 } };

        private Ticket() { }

        public static (Ticket? ticket, string? error) Create(Guid organizationId, string title, string? description, string priority, Guid creatorId, Guid? assigneeId = null, decimal? slaTargetHours = null)
        {
            title = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title) || title.Length < MinTitleLength || title.Length > MaxTitleLength)
                return (null, $"Заголовок от {MinTitleLength} до {MaxTitleLength} символов");
            if (!string.IsNullOrWhiteSpace(description) && description.Length > MaxDescriptionLength)
                return (null, $"Описание максимум {MaxDescriptionLength} символов");
            priority = priority.Trim();
            if (!ValidPriorities.Contains(priority)) return (null, "Недопустимый приоритет");
            if (creatorId == Guid.Empty) return (null, "Некорректный создатель");

            var target = slaTargetHours ?? DefaultSLATargets.GetValueOrDefault(priority, 8);
            return (new Ticket
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Title = title,
                Description = description?.Trim(),
                Priority = priority,
                Status = "Open",
                CreatorId = creatorId,
                AssigneeId = assigneeId,
                SLATargetHours = target,
                IsSLABreached = false,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        public void AssignTo(Guid? assigneeId)
        {
            if (Status != "Open" && Status != "InProgress") throw new InvalidOperationException("Нельзя назначить тикет в текущем статусе");
            AssigneeId = assigneeId;
            if (assigneeId.HasValue && Status == "Open") Status = "InProgress";
            MarkAsUpdated();
        }

        public void Resolve()
        {
            if (Status is "Resolved" or "Closed") throw new InvalidOperationException($"Тикет уже в статусе {Status}");
            ResolvedAt = DateTime.UtcNow; Status = "Resolved"; CheckSLA(); MarkAsUpdated();
        }

        public void Close() { if (Status != "Resolved") throw new InvalidOperationException("Закрыть можно только решённый тикет"); Status = "Closed"; MarkAsUpdated(); }
        public void Reopen() { if (Status is not "Resolved" and not "Closed") throw new InvalidOperationException("Переокрыть можно только решённый/закрытый тикет"); Status = "InProgress"; ResolvedAt = null; IsSLABreached = false; MarkAsUpdated(); }

        public void CheckSLA()
        {
            var elapsed = (ResolvedAt ?? DateTime.UtcNow) - CreatedAt;
            IsSLABreached = elapsed.TotalHours > (double)SLATargetHours;
        }

        public decimal GetTotalLoggedHours() => TimeLogs.Sum(tl => tl.DurationHours);
    }
}
