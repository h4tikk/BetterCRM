using BetterCRM.Core.Extensions;

namespace BetterCRM.Core.Models
{
    public class Ticket : TenantEntity
    {
        public string Title { get; internal set; } = string.Empty;
        public string? Description { get; internal set; }


        public TicketPriority Priority { get; internal set; }
        public TicketStatus Status { get; internal set; } = TicketStatus.Draft;

        public Guid CreatorId { get; internal set; }
        public Guid? AssigneeId { get; internal set; }

        public Guid? DepartmentId { get; internal set; }

        public DateTime? ResolvedAt { get; internal set; }

        public DateTime? ClosedAt { get; internal set; }

        public decimal SLATargetHours { get; internal set; }
        public bool IsSLABreached { get; internal set; }

        public decimal OverduePenaltyHours { get; internal set; } = 0;

        public User Creator { get; internal set; } = null!;
        public User? Assignee { get; internal set; }
        public ICollection<TicketParticipant> Participants { get; internal set; } = new List<TicketParticipant>();
        public ICollection<TimeLog> TimeLogs { get; internal set; } = new List<TimeLog>();

        public const int MinTitleLength = 1;
        public const int MaxTitleLength = 200;
        public const int MaxDescriptionLength = 1000;

        public static readonly Dictionary<TicketPriority, decimal> DefaultSLATargets = new()
        {
            [TicketPriority.High] = 4,
            [TicketPriority.Medium] = 8,
            [TicketPriority.Low] = 24
        };

        public static readonly Dictionary<TicketPriority, decimal> OverduePenalties = new()
        {
            [TicketPriority.High] = 2,
            [TicketPriority.Medium] = 1,
            [TicketPriority.Low] = 0.5m
        };

        private Ticket() { }

        public static (Ticket? ticket, string? error) Create(
            Guid organizationId, string title, string? description,
            TicketPriority priority, Guid creatorId,
            Guid? departmentId = null, Guid? assigneeId = null,
            decimal? slaTargetHours = null)
        {
            title = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title) || title.Length < MinTitleLength || title.Length > MaxTitleLength)
                return (null, $"Заголовок от {MinTitleLength} до {MaxTitleLength} символов");
            if (!string.IsNullOrWhiteSpace(description) && description.Length > MaxDescriptionLength)
                return (null, $"Описание максимум {MaxDescriptionLength} символов");
            if (creatorId == Guid.Empty)
                return (null, "Некорректный создатель");

            var target = slaTargetHours ?? DefaultSLATargets[priority];

            return (new Ticket
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Title = title,
                Description = description?.Trim(),
                Priority = priority,
                Status = TicketStatus.Draft,
                CreatorId = creatorId,
                DepartmentId = departmentId,
                AssigneeId = assigneeId,
                SLATargetHours = target,
                IsSLABreached = false,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        public void Approve()
        {
            if (Status != TicketStatus.Draft)
                throw new InvalidOperationException("Одобрить можно только черновик");
            Status = TicketStatus.Open;
            MarkAsUpdated();
        }

        public void Reject()
        {
            if (Status != TicketStatus.Draft)
                throw new InvalidOperationException("Отклонить можно только черновик");
            Status = TicketStatus.Closed;
            ClosedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void AssignTo(Guid? assigneeId)
        {
            if (Status != TicketStatus.Open && Status != TicketStatus.InProgress)
                throw new InvalidOperationException("Нельзя назначить тикет в текущем статусе");
            AssigneeId = assigneeId;
            if (assigneeId.HasValue && Status == TicketStatus.Open)
                Status = TicketStatus.InProgress;
            MarkAsUpdated();
        }

        public void Resolve()
        {
            if (Status is TicketStatus.Resolved or TicketStatus.Closed)
                throw new InvalidOperationException($"Тикет уже в статусе {Status}");
            ResolvedAt = DateTime.UtcNow;
            Status = TicketStatus.Resolved;
            CheckSLA();
            MarkAsUpdated();
        }

        public void Close()
        {
            if (Status != TicketStatus.Resolved)
                throw new InvalidOperationException("Закрыть можно только решённый тикет");
            Status = TicketStatus.Closed;
            ClosedAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void Reopen()
        {
            if (Status is not TicketStatus.Resolved and not TicketStatus.Closed)
                throw new InvalidOperationException("Переоткрыть можно только решённый/закрытый тикет");
            Status = TicketStatus.InProgress;
            ResolvedAt = null;
            ClosedAt = null;
            IsSLABreached = false;
            MarkAsUpdated();
        }

        public void CheckSLA()
        {
            var elapsed = (ResolvedAt ?? DateTime.UtcNow) - CreatedAt;
            IsSLABreached = elapsed.TotalHours > (double)SLATargetHours;
        }

        public void ApplyOverduePenalty()
        {
            if (!IsSLABreached)
                throw new InvalidOperationException("SLA не нарушен — штраф начислять не нужно");
            if (OverduePenaltyHours > 0)
                throw new InvalidOperationException("Штраф уже начислен");
            OverduePenaltyHours = OverduePenalties[Priority];
            MarkAsUpdated();
        }

        public decimal GetTotalLoggedHours() =>
            TimeLogs.Sum(tl => tl.DurationHours);
    }
}