namespace BetterCRM.Core.Models
{
    public class PayrollRecord : BaseEntity
    {
        public Guid UserId { get; private set; }
        public DateTime PeriodStart { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        public decimal TotalHours { get; private set; }
        public decimal HourlyRate { get; private set; }
        public decimal CalculatedSalary { get; private set; }
        public string Status { get; private set; } = string.Empty;

        public User User { get; private set; } = null!;

        public static readonly string[] ValidStatuses = { "Calculated", "Approved", "Paid" };
        public const decimal MinHourlyRate = 0;
        public const decimal MaxHourlyRate = 10000;
        public const decimal MinTotalHours = 0;
        public const decimal MaxTotalHours = 744;

        private PayrollRecord() { }

        public static (PayrollRecord? record, string? error) Create(
            Guid userId,
            DateTime periodStart,
            DateTime periodEnd,
            decimal totalHours,
            decimal hourlyRate)
        {
            if (userId == Guid.Empty)
                return (null, "Некорректный ID пользователя");

            if (periodStart >= periodEnd)
                return (null, "Период должен быть корректным (start < end)");

            if (totalHours < MinTotalHours || totalHours > MaxTotalHours)
                return (null, $"Отработано часов должно быть от {MinTotalHours} до {MaxTotalHours}");

            if (hourlyRate < MinHourlyRate || hourlyRate > MaxHourlyRate)
                return (null, $"Ставка должна быть от {MinHourlyRate} до {MaxHourlyRate}");

            var salary = Math.Round(totalHours * hourlyRate, 2);

            return (new PayrollRecord
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PeriodStart = periodStart.Date,
                PeriodEnd = periodEnd.Date,
                TotalHours = Math.Round(totalHours, 2),
                HourlyRate = Math.Round(hourlyRate, 2),
                CalculatedSalary = salary,
                Status = "Calculated",
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        public void ChangeStatus(string newStatus)
        {
            if (!ValidStatuses.Contains(newStatus))
                throw new InvalidOperationException($"Недопустимый статус: {newStatus}");

            if (Status == "Paid" && newStatus != "Paid")
                throw new InvalidOperationException("Оплаченный расчёт нельзя изменить");

            Status = newStatus;
            MarkAsUpdated();
        }

        public void Approve() => ChangeStatus("Approved");
        public void MarkAsPaid() => ChangeStatus("Paid");

        public bool IsForPeriod(DateTime date) => date >= PeriodStart && date <= PeriodEnd;
        public bool IsForUser(Guid userId) => UserId == userId;
    }
}
