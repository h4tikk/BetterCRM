namespace BetterCRM.DataAccess.Entities
{
    public class TimeLogEntity : TenantDbEntity
    {
        public Guid WorkSessionId { get; set; }
        public WorkSessionEntity WorkSession { get; set; } = null!;
        public Guid TicketId { get; set; }
        public TicketEntity Ticket { get; set; } = null!;
        public decimal DurationHours { get; set; }
        public string? Description { get; set; }
    }
}
