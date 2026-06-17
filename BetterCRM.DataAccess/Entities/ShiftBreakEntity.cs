namespace BetterCRM.DataAccess.Entities
{
    public class ShiftBreakEntity : TenantDbEntity
    {
        public Guid ShiftId { get; set; }
        public ShiftEntity Shift { get; set; } = null!;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Type { get; set; } = "Custom";
        public bool IsPaid { get; set; }
    }
}
