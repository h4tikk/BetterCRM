namespace BetterCRM.DataAccess.Entities
{
    public class WorkSessionEntity : TenantDbEntity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public Guid? ShiftId { get; set; }
        public ShiftEntity? Shift { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string? Comment { get; set; }
        public ICollection<TimeLogEntity> TimeLogs { get; set; } = new List<TimeLogEntity>();
    }
}
