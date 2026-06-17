namespace BetterCRM.Core.Models
{
    public class TenantEntity : BaseEntity
    {
         public Guid OrganizationId { get; internal set; }
    }
}
