namespace BetterCRM.DataAccess.Entities
{
    public class BaseDbEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
