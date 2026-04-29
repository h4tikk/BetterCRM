namespace BetterCRM.Core.Models
{
    public class TicketParticipant : TenantEntity
    {
        public Guid TicketId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime JoinedAt { get; private set; }
        public string Role { get; private set; } = string.Empty;

        public Ticket Ticket { get; private set; } = null!;
        public User User { get; private set; } = null!;

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

