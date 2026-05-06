namespace BetterCRM.DataAccess.Entities
{
    public class ShiftEntity : TenantDbEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public decimal LatenessPenaltyHours { get; set; } = 0;
        public decimal EarlyLeavePenaltyHours { get; set; } = 0;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = "Scheduled";
    }
}
