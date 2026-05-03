namespace BetterCRM.Core.Models
{
    public class TicketParticipant : TenantEntity
    {
        public Guid TicketId { get; internal set; }
        public Guid UserId { get; internal set; }
        public DateTime JoinedAt { get; internal set; }
        public string Role { get; internal set; } = string.Empty;

        public Ticket Ticket { get; internal set; } = null!;
        public User User { get; internal set; } = null!;

        public static readonly string[] ValidRoles = { "Worker", "Reviewer", "Observer" };
        private TicketParticipant() { }

        public static (TicketParticipant? p, string? error) Create(Guid organizationId, Guid ticketId, Guid userId, string role = "Worker")
        {
            if (ticketId == Guid.Empty || userId == Guid.Empty) 
                return (null, "Некорректные ID");
            if (!ValidRoles.Contains(role)) 
                return (null, $"Недопустимая роль. Доступные: {string.Join(", ", ValidRoles)}");
            return (new TicketParticipant { Id = Guid.NewGuid(), OrganizationId = organizationId, TicketId = ticketId, UserId = userId, Role = role, JoinedAt = DateTime.UtcNow }, null);
        }

        public void ChangeRole(string newRole) { if (!ValidRoles.Contains(newRole)) throw new InvalidOperationException("Недопустимая роль"); Role = newRole; MarkAsUpdated(); }
    }

}

