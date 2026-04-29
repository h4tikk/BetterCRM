namespace BetterCRM.Core.Models
{
    public class Position : TenantEntity
    {
        public string Title { get; private set; } = string.Empty;
        public decimal HourlyRate { get; private set; }
        public int DailyNormHours { get; private set; }


        public ICollection<User> Users { get; private set; } = new List<User>();

        public const int MinTitleLength = 4;
        public const int MaxTitleLength = 100;
        public const decimal MinHourlyRate = 0;
        public const decimal MaxHourlyRate = 10000;
        public const int MaxDailyNorm = 16;
        public const int MinDailyNorm = 1;

        private Position() { }

        public static (Position? position, string? error) Create(Guid organizationId,string title, decimal hourlyRate, int dailyHours = 8)
        {
            if (string.IsNullOrWhiteSpace(title))
                return (null, "Название должности не может быть пустым");

            title = title.Trim();
            if (title.Length < MinTitleLength || title.Length > MaxTitleLength)
                return (null, $"Название должно содержать от {MinTitleLength} до {MaxTitleLength} символов");

            if (hourlyRate < MinHourlyRate || hourlyRate > MaxHourlyRate)
                return (null, $"Ставка должна быть от {MinHourlyRate} до {MaxHourlyRate}");

            return (new Position
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                Title = title,
                HourlyRate = Math.Round(hourlyRate, 2),
                DailyNormHours = dailyHours
            }, null);
        }
        public void UpdateRate(decimal newRate)
        {
            if (newRate < MinHourlyRate || newRate > MaxHourlyRate)
                throw new InvalidOperationException($"Ставка должна быть от {MinHourlyRate} до {MaxHourlyRate}");

            HourlyRate = Math.Round(newRate, 2);
            MarkAsUpdated();
        }

        public void UpdateTitle(string newTitle)
        {
            var (_, error) = Create(OrganizationId ,newTitle, HourlyRate);
            if (error != null) throw new InvalidOperationException(error);

            Title = newTitle.Trim();
            MarkAsUpdated();
        }
        public void UpdateDailyNorm(int newNorm)
        {
            if (newNorm < MinDailyNorm || newNorm > MaxDailyNorm)
                throw new InvalidOperationException($"Норма от {MinDailyNorm} до {MaxDailyNorm} часов");
            DailyNormHours = newNorm;
            MarkAsUpdated();
        }
        public int GetExpectedHoursForPeriod(DateTime from, DateTime to)
        {
            var workDays = 0;
            var current = from.Date;
            while (current <= to)
            {
                if (current.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday) workDays++;
                current = current.AddDays(1);
            }
            return workDays * DailyNormHours;
        }
    }
}
