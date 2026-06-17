namespace BetterCRM.DataAccess.Entities
{
    public class TenantDbEntity : BaseDbEntity
    {
        public Guid OrganizationId { get; set; }
        public OrganizationEntity Organization { get; set; } = null!;
    }
}
