using BetterCRM.Core.Extensions;

namespace BetterCRM.Core.Models
{
    public class Shift : TenantEntity
    {
        public Guid UserId { get; internal set; }
        public User User { get; internal set; } = null!;
        public DateTime Date { get; internal set; }
        public TimeSpan StartTime { get; internal set; }
        public TimeSpan EndTime { get; internal set; }

        // ✅ ИЗМЕНЕНО: string → enum
        public ShiftStatus Status { get; internal set; } = ShiftStatus.Scheduled;

        // ✅ НОВОЕ: штрафные часы за посещаемость
        public decimal LatenessPenaltyHours { get; internal set; } = 0;
        public decimal EarlyLeavePenaltyHours { get; internal set; } = 0;

        private Shift() { }

        public static (Shift? shift, string? error) Create(
            Guid organizationId, Guid userId,
            DateTime date, TimeSpan start, TimeSpan end)
        {
            if (userId == Guid.Empty)
                return (null, "Некорректный пользователь");
            if (end <= start)
                return (null, "Время окончания должно быть позже начала");
            if (date < DateTime.UtcNow.Date.AddDays(-1))
                return (null, "Нельзя создавать смены в прошлом");

            return (new Shift
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = userId,
                Date = date.Date,
                StartTime = start,
                EndTime = end,
                Status = ShiftStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        public void Cancel()
        {
            if (Status == ShiftStatus.Completed)
                throw new InvalidOperationException("Нельзя отменить завершённую смену");
            Status = ShiftStatus.Cancelled;
            MarkAsUpdated();
        }

        // ✅ ИСПРАВЛЕНО: было "Complete" — опечатка, должно быть "Completed"
        public void Complete()
        {
            if (Status == ShiftStatus.Cancelled)
                throw new InvalidOperationException("Нельзя завершить отменённую смену");
            Status = ShiftStatus.Completed;
            MarkAsUpdated();
        }

        public void UpdateTime(TimeSpan start, TimeSpan end)
        {
            if (end <= start)
                throw new InvalidOperationException("Время начала позже времени окончания");
            if (Status != ShiftStatus.Scheduled)
                throw new InvalidOperationException("Изменить время можно только у запланированной смены");
            StartTime = start;
            EndTime = end;
            // ✅ ИСПРАВЛЕНО: MarkAsUpdated() отсутствовал в оригинале
            MarkAsUpdated();
        }


        public void ApplyAttendancePenalty(decimal latenessHours, decimal earlyLeaveHours)
        {
            if (latenessHours < 0 || earlyLeaveHours < 0)
                throw new InvalidOperationException("Штрафные часы не могут быть отрицательными");
            LatenessPenaltyHours = Math.Round(latenessHours, 2);
            EarlyLeavePenaltyHours = Math.Round(earlyLeaveHours, 2);
            MarkAsUpdated();
        }

        public decimal GetScheduledHours() =>
            (decimal)(EndTime - StartTime).TotalHours;

        public decimal GetTotalPenaltyHours() =>
            LatenessPenaltyHours + EarlyLeavePenaltyHours;
    }
}