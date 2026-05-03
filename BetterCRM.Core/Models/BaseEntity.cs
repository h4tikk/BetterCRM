namespace BetterCRM.Core.Models
{
    public abstract class BaseEntity
    {
        public Guid Id { get; internal set; }
        public DateTime CreatedAt { get; internal set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; internal set; }

        protected void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

    }
}
