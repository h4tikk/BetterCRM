using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.Core.Models
{
    public class Ticket : BaseEntity
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
        public ICollection<WorkSession> WorkSessions { get; private set; } = new List<WorkSession>();

        public const int MinTitleLength = 1;
        public const int MaxTitleLength = 200;
        public const int MaxDescriptionLength = 1000;
        public static readonly string[] ValidPriorities = { "High", "Medium", "Low" };
        public static readonly string[] ValidStatuses = { "Open", "InProgress", "Resolved", "Closed" };
        public static readonly Dictionary<string, decimal> DefaultSLATargets = new()
    {
        { "High", 4 }, { "Medium", 8 }, { "Low", 24 }
    };

        private Ticket() { }

        public static (Ticket? ticket, string? error) Create(
            string title,
            string? description,
            string priority,
            Guid creatorId,
            Guid? assigneeId = null,
            decimal? slaTargetHours = null)
        {
            title = title?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(title) || title.Length < MinTitleLength || title.Length > MaxTitleLength)
                return (null, $"Заголовок должен содержать от {MinTitleLength} до {MaxTitleLength} символов");

            if (!string.IsNullOrWhiteSpace(description) && description.Length > MaxDescriptionLength)
                return (null, $"Описание не может превышать {MaxDescriptionLength} символов");

            priority = priority.Trim();
            if (!ValidPriorities.Contains(priority))
                return (null, $"Недопустимый приоритет. Доступные: {string.Join(", ", ValidPriorities)}");

            if (creatorId == Guid.Empty)
                return (null, "Некорректный ID создателя");

            var targetHours = slaTargetHours ?? DefaultSLATargets.GetValueOrDefault(priority, 8);
            if (targetHours <= 0 || targetHours > 168) 
                return (null, "SLA target должен быть от 1 до 168 часов");

            return (new Ticket
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description?.Trim(),
                Priority = priority,
                Status = "Open",
                CreatorId = creatorId,
                AssigneeId = assigneeId,
                SLATargetHours = targetHours,
                IsSLABreached = false,
                CreatedAt = DateTime.UtcNow
            }, null);
        }


        public void AssignTo(Guid? assigneeId)
        {
            if (Status != "Open" && Status != "InProgress")
                throw new InvalidOperationException("Нельзя назначить тикет в текущем статусе");

            AssigneeId = assigneeId;
            if (assigneeId.HasValue && Status == "Open")
                Status = "InProgress";

            MarkAsUpdated();
        }

        public void ChangePriority(string newPriority)
        {
            if (!ValidPriorities.Contains(newPriority))
                throw new InvalidOperationException($"Недопустимый приоритет: {newPriority}");

            Priority = newPriority;
            SLATargetHours = DefaultSLATargets.GetValueOrDefault(newPriority, 8);
            CheckSLA(); 
            MarkAsUpdated();
        }

        public void ChangeStatus(string newStatus)
        {
            if (!ValidStatuses.Contains(newStatus))
                throw new InvalidOperationException($"Недопустимый статус: {newStatus}");

            if (Status == "Resolved" && newStatus != "Closed")
                throw new InvalidOperationException("Решённый тикет можно только закрыть");

            if (Status == "Closed")
                throw new InvalidOperationException("Закрытый тикет нельзя изменить");

            Status = newStatus;

            if (newStatus == "Resolved" && ResolvedAt == null)
            {
                ResolvedAt = DateTime.UtcNow;
                CheckSLA();
            }

            MarkAsUpdated();
        }

        public void Resolve()
        {
            if (Status is "Resolved" or "Closed")
                throw new InvalidOperationException($"Тикет уже в статусе {Status}");

            ResolvedAt = DateTime.UtcNow;
            Status = "Resolved";
            CheckSLA();
            MarkAsUpdated();
        }

        public void Close()
        {
            if (Status != "Resolved")
                throw new InvalidOperationException("Закрыть можно только решённый тикет");

            Status = "Closed";
            MarkAsUpdated();
        }

        public void Reopen()
        {
            if (Status != "Resolved" && Status != "Closed")
                throw new InvalidOperationException("Переокрыть можно только решённый/закрытый тикет");

            Status = "InProgress";
            ResolvedAt = null;
            IsSLABreached = false;
            MarkAsUpdated();
        }

        public void CheckSLA()
        {
            if (Status == "Resolved" && ResolvedAt.HasValue)
            {
                var elapsedHours = (ResolvedAt.Value - CreatedAt).TotalHours;
                IsSLABreached = elapsedHours > (double)SLATargetHours;
            }
            else if (Status is "Open" or "InProgress")
            {
                var elapsedHours = (DateTime.UtcNow - CreatedAt).TotalHours;
                IsSLABreached = elapsedHours > (double)SLATargetHours;
            }
        }

        public bool CanBeModifiedBy(User user)
        {
            return user.Role == "Admin" ||
                   user.Id == CreatorId ||
                   user.Id == AssigneeId ||
                   (user.Role == "DepartmentHead" && user.DepartmentId == Assignee?.DepartmentId);
        }

        public TimeSpan GetElapsedTime() =>
            ResolvedAt.HasValue ? ResolvedAt.Value - CreatedAt : DateTime.UtcNow - CreatedAt;

        public decimal GetElapsedHours() => (decimal)GetElapsedTime().TotalHours;

        public bool IsOverdue() => GetElapsedHours() > SLATargetHours && Status is "Open" or "InProgress";
    }
}
