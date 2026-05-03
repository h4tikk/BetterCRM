namespace BetterCRM.DataAccess.Entities
{
    public class ShiftEntity : TenantDbEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = "Scheduled";
    }
}
