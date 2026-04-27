namespace BetterCRM.Core.Models
{
    public class Position : BaseEntity
    {
        public string Title { get; private set; } = string.Empty;
        public decimal HourlyRate { get; private set; }


        public ICollection<User> Users { get; private set; } = new List<User>();

        public const int MinTitleLength = 4;
        public const int MaxTitleLength = 100;
        public const decimal MinHourlyRate = 0;
        public const decimal MaxHourlyRate = 10000;

        private Position() { }

        public static (Position? position, string? error) Create(string title, decimal hourlyRate)
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
                Title = title,
                HourlyRate = Math.Round(hourlyRate, 2)
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
            var (_, error) = Create(newTitle, HourlyRate);
            if (error != null) throw new InvalidOperationException(error);

            Title = newTitle.Trim();
            MarkAsUpdated();
        }

    }
}
