using System.ComponentModel.DataAnnotations.Schema;

namespace BetterCRM.Core.Models
{
    public class WorkSession : TenantEntity
    {
        public Guid UserId { get; internal set; }
        public User User { get; internal set; } = null!;
        public Guid? ShiftId { get; internal set; }
        public Shift? Shift { get; internal set; }
        public DateTime StartedAt { get; internal set; }
        public DateTime? EndedAt { get; internal set; }
        public string? Comment { get; internal set; }

        public ICollection<TimeLog> TimeLogs { get; internal set; } = new List<TimeLog>();

        public const int MaxCommentLength = 300;
        public const int MaxSessionHours = 24;

        private WorkSession() { }

        public static (WorkSession? workSession, string? error) Start(Guid organizationId, Guid userId, Guid? shiftId = null)
        {
            if (userId == Guid.Empty) return (null, "Некорректный ID пользователя");
            return (new WorkSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrganizationId = organizationId,
                ShiftId = shiftId,
                StartedAt = DateTime.UtcNow

            }, null);
        }

        public (bool success, string? error) Stop(string? comment = null)
        {
            if (EndedAt.HasValue) return (false, "Сессия уже завершена");
            if (!string.IsNullOrWhiteSpace(comment) && comment.Length > MaxCommentLength)
                return (false, $"Комментарий максимум {MaxCommentLength} символов");

            var ended = DateTime.UtcNow;
            if ((ended - StartedAt).TotalHours > MaxSessionHours)
                return (false, $"Сессия не может длиться более {MaxSessionHours} часов");

            EndedAt = ended; Comment = comment?.Trim(); MarkAsUpdated();
            return (true, null);
        }

        [NotMapped] public decimal DurationHours => EndedAt.HasValue ? (decimal)(EndedAt.Value - StartedAt).TotalHours : 0m;
        [NotMapped] public bool IsActive => !EndedAt.HasValue;

    }
}