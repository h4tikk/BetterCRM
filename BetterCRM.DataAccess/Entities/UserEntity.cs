
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Entities
{
    internal class UserEntity : TenantDbEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;
        public Guid departmentId {  get; set; }
        public DepartmentEntity? Department { get; set; }
        public Guid PositionId { get; set; }
        public PositionEntity Position { get; set; } = null!;
        public DateTime HireDate { get; set; }
        
    }
}
