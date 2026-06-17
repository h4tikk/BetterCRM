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

        public ShiftStatus Status { get; internal set; } = ShiftStatus.Scheduled;

        public decimal LatenessPenaltyHours { get; internal set; } = 0;
        public decimal EarlyLeavePenaltyHours { get; internal set; } = 0;

        public ICollection<ShiftBreak> Breaks { get; internal set; } = new List<ShiftBreak>();

        private Shift() { }

        public static (Shift? shift, string? error) Create(
            Guid organizationId, Guid userId,
            DateTime date, TimeSpan start, TimeSpan end)
        {
            if (userId == Guid.Empty)
                return (null, "Некорректный пользователь");
            if (end <= start)
                return (null, "Время окончания должно быть позже начала");
            const int AllowedBackfillDays = 1;
            if (date < DateTime.UtcNow.Date.AddDays(-AllowedBackfillDays))
                return (null, $"Нельзя создавать смены ранее чем {AllowedBackfillDays} день назад");

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

        public (ShiftBreak? shiftBreak, string? error) AddBreak(TimeSpan start, TimeSpan end, BreakType type, bool isPaid)
        {
            if (Status != ShiftStatus.Scheduled)
                return (null, "Добавить перерыв можно только в запланированную смену");
            if (start < StartTime || end > EndTime)
                return (null, "Перерыв должен находиться в пределах смены");
            if (Breaks.Any(b => start < b.EndTime && end > b.StartTime))
                return (null, "Перерыв пересекается с уже существующим");

            var (shiftBreak, err) = ShiftBreak.Create(OrganizationId, Id, start, end, type, isPaid);
            if (shiftBreak == null) return (null, err);

            Breaks.Add(shiftBreak);
            MarkAsUpdated();
            return (shiftBreak, null);
        }

        public decimal GetScheduledHours() =>
            (decimal)(EndTime - StartTime).TotalHours;

        public decimal GetBreakHours(bool paidOnly = false) =>
            Breaks.Where(b => !paidOnly || b.IsPaid).Sum(b => b.DurationHours);

        public decimal GetPaidScheduledHours() =>
            GetScheduledHours() - Breaks.Where(b => !b.IsPaid).Sum(b => b.DurationHours);

        public decimal GetTotalPenaltyHours() =>
            LatenessPenaltyHours + EarlyLeavePenaltyHours;
    }
}