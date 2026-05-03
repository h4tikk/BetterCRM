namespace BetterCRM.DataAccess.Entities
{
    public class TicketEntity : TenantDbEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid CreatorId { get; set; }
        public UserEntity Creator { get; set; } = null!;
        public Guid? AssigneeId { get; set; }
        public UserEntity? Assignee { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public decimal SLATargetHours { get; set; }
        public bool IsSLABreached { get; set; }

        public ICollection<TicketParticipantEntity> Participants { get; set; } = new List<TicketParticipantEntity>();
        public ICollection<TimeLogEntity> TimeLogs { get; set; } = new List<TimeLogEntity>();
    }
}
