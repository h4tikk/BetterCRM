namespace BetterCRM.Core.Models
{
    public class Shift : TenantEntity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public DateTime Date { get; private set;  }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public string Status { get; private set; } = "Scheduled";

        private Shift() { }

        public static (Shift? shift, string? error) Create(Guid organizationId, Guid userId, DateTime date, TimeSpan start, TimeSpan end)
        {
            if (userId == Guid.Empty) return (null, "Некорректный пользователь");
            if (end <= start) return (null, "Время окончания должно быть позже начала");
            if (date < DateTime.UtcNow.Date.AddDays(-1)) return (null, "Нельзя создавать смены в прошлом");

            return (new Shift
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = userId,
                Date = date.Date,
                StartTime = start,
                EndTime = end,
                Status = "Scheduled"
            }, null);
        }
        public void Cancel() { Status = "Cancelled"; MarkAsUpdated();  }
        public void Complete() { Status = "Complete"; MarkAsUpdated(); }
        public void UpdateTime(TimeSpan start, TimeSpan end)
        {
            if (end <= start) throw new InvalidOperationException("Время начала позже времени окончания");
            StartTime = start;
            EndTime = end;
        }
        public decimal GetScheduledHours() => (decimal)(EndTime - StartTime).TotalHours;
        
    }
}
