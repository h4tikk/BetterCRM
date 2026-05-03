namespace BetterCRM.DataAccess.Entities
{
    public class PayrollRecordEntity : TenantDbEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal ScheduledHours { get; set; }
        public decimal ActualHours { get; set; }
        public decimal PenaltyHours { get; set; }
        public decimal BillableHours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal CalculatedSalary { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
