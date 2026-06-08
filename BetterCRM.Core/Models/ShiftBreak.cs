using BetterCRM.Core.Extensions;

namespace BetterCRM.Core.Models
{
    public class ShiftBreak : TenantEntity
    {
        public Guid ShiftId { get; internal set; }
        public TimeSpan StartTime { get; internal set; }
        public TimeSpan EndTime { get; internal set; }
        public BreakType Type { get; internal set; }
        public bool IsPaid { get; internal set; }

        private ShiftBreak() { }

        public static (ShiftBreak? shiftBreak, string? error) Create(
            Guid organizationId, Guid shiftId,
            TimeSpan start, TimeSpan end,
            BreakType type, bool isPaid)
        {
            if (shiftId == Guid.Empty) return (null, "Некорректная смена");
            if (end <= start) return (null, "Время окончания перерыва должно быть позже начала");

            return (new ShiftBreak
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                ShiftId = shiftId,
                StartTime = start,
                EndTime = end,
                Type = type,
                IsPaid = isPaid,
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        public decimal DurationHours => (decimal)(EndTime - StartTime).TotalHours;
    }
}
