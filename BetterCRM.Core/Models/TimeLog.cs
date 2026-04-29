namespace BetterCRM.Core.Models
{
    public class TimeLog : TenantEntity
    {
        public Guid WorkSessionId { get; private set; }
        public WorkSession WorkSession { get; private set; } = null!;
        public Guid TicketId { get; private set; }
        public Ticket Ticket { get; private set; } = null!;
        public decimal DurationHours { get; private set; }
        public string? Description { get; private set; }

        private TimeLog() { }

        public static (TimeLog? log, string? error) Create(Guid organizationId, Guid workSessionId, Guid ticketId, decimal durationHours, string? description = null)
        {
            if (durationHours <= 0) return (null, "Длительность должна быть больше 0");
            if (durationHours > 12) return (null, "Нельзя списать более 12 часов за один лог");

            return (new TimeLog
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                WorkSessionId = workSessionId,
                TicketId = ticketId,
                DurationHours = Math.Round(durationHours, 2),
                Description = description?.Trim()
            }, null);
        }
    }
}
