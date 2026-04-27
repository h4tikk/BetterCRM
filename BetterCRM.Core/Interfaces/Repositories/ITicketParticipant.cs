using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface ITicketParticipant : IRepository<TicketParticipant>
    {
        Task<bool> IsParticipantAsync(Guid ticketId, Guid userId);
        Task<List<TicketParticipant>> GetByTicketAync(Guid ticketId);
        Task<List<TicketParticipant>> GetByUserAsync(Guid userId);
        Task<List<TicketParticipant>> GetByTicketAndRoleAsync(Guid ticketId, string role);
        Task RemoveByTicketAndUserAsync(Guid ticketId, Guid userId);
        Task UpdateRoleAsync(Guid ticketId, Guid userId, string newRole);

    }
}
