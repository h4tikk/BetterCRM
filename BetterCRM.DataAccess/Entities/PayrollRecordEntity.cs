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

        public decimal AttendancePenaltyHours { get; set; } = 0;
        public decimal TicketPenaltyHours { get; set; } = 0;
        public decimal TotalPenaltyHours { get; set; } = 0;
        public decimal FinalBillableHours { get; set; } = 0;
        public decimal HourlyRate { get; set; }
        public decimal CalculatedSalary { get; set; }
        public string Status { get; set; } = "Calculated";
    }
}