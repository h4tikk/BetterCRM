namespace BetterCRM.Core.Models
{
    public class TicketParticipant : BaseEntity
    {
        public Guid TicketId { get; private set; }
        public Guid UserId { get; private set; }
        public DateTime JoinedAt { get; private set; }
        public string Role {  get; private set; } = string.Empty;

        public Ticket Ticket { get; private set; } = null!;
        public User User { get; private set; } = null!;

        public static readonly string[] ValidRoles = { "Worker", "Reviewer", "Observer" };

        private TicketParticipant() { }

        public static (TicketParticipant? participant, string? error) Create(
            Guid ticketId,
            Guid userId,
            string role = "Worker")
        {
            if (ticketId == Guid.Empty) return (null, "Некорректный ID тикета");

            if (userId == Guid.Empty) return (null, "Некорретный ID пользователя");

            if (!ValidRoles.Contains(role)) return (null, $"Недопустимая роль. Домтупные: {string.Join(", ", ValidRoles)}");

            return (new TicketParticipant
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                UserId = userId,
                Role = role,
                JoinedAt = DateTime.UtcNow
            }, null);
        }
        public void ChangeRole(string role)
        {
            if (!ValidRoles.Contains(role))
                throw new InvalidOperationException($"Недопустимая роль: {role}");
            Role = role;
            MarkAsUpdated();
        }

    }
}
