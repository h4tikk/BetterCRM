namespace BetterCRM.DataAccess.Entities
{
    public class DepartmentEntity : TenantDbEntity
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
