namespace BetterCRM.DataAccess.Entities
{
    public class TicketParticipantEntity : TenantDbEntity
    {
        public Guid TicketId { get; set; }
        public TicketEntity Ticket { get; set; } = null!;
        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
        public DateTime JoinedAt { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}
