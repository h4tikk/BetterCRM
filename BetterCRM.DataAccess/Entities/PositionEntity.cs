namespace BetterCRM.DataAccess.Entities
{
    public class PositionEntity : TenantDbEntity
    {
        public string Title { get; set; } = string.Empty;
        public decimal HourlyRate { get; set; }
        public int DailyNormHours { get; set; }
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
