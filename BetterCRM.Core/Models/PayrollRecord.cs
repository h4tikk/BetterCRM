namespace BetterCRM.Core.Models
{
    public class PayrollRecord : TenantEntity
    {
        public Guid UserId { get; private set; }
        public DateTime PeriodStart { get; private set; }
        public DateTime PeriodEnd { get; private set; }

        public decimal ScheduledHours { get; private set; }   
        public decimal ActualHours { get; private set; }      
        public decimal PenaltyHours { get; private set; }    
        public decimal BillableHours { get; private set; }    
        public decimal HourlyRate { get; private set; }
        public decimal CalculatedSalary { get; private set; }
        public string Status { get; private set; } = string.Empty;

        public User User { get; private set; } = null!;

        public static readonly string[] ValidStatuses = { "Calculated", "Approved", "Paid" };
        private PayrollRecord() { }

        public static (PayrollRecord? rec, string? error) Create(Guid organizationId, Guid userId, DateTime periodStart, DateTime periodEnd, decimal scheduled, decimal actual, decimal penalty, decimal billable, decimal rate)
        {
            if (userId == Guid.Empty || periodStart >= periodEnd) return (null, "Некорректный период или пользователь");
            var salary = Math.Round(billable * rate, 2);
            return (new PayrollRecord
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = userId,
                PeriodStart = periodStart.Date,
                PeriodEnd = periodEnd.Date,
                ScheduledHours = Math.Round(scheduled, 2),
                ActualHours = Math.Round(actual, 2),
                PenaltyHours = Math.Round(penalty, 2),
                BillableHours = Math.Round(billable, 2),
                HourlyRate = Math.Round(rate, 2),
                CalculatedSalary = salary,
                Status = "Calculated",
                CreatedAt = DateTime.UtcNow
            }, null);
        }

        public void ChangeStatus(string newStatus) 
        {
            if (!ValidStatuses.Contains(newStatus)) 
                throw new InvalidOperationException("Недопустимый статус"); 
            if (Status == "Paid" && newStatus != "Paid") 
                throw new InvalidOperationException("Оплаченный расчёт нельзя изменить");
            Status = newStatus; MarkAsUpdated(); 
        }
        public void Approve() => ChangeStatus("Approved");
        public void MarkAsPaid() => ChangeStatus("Paid");
    }
}
