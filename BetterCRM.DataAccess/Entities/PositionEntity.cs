namespace BetterCRM.DataAccess.Entities
{
    public class PositionEntity : TenantDbEntity
    {
        public string Title { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public int DailyNormHours { get; set; }
        public Guid DepartmentId {  get; set; }
        public DepartmentEntity Department { get; set; } = null!;
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
