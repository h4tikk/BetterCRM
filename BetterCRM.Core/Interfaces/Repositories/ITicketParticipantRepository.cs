using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface ITicketParticipantRepository : IRepository<TicketParticipant>
    {
        Task<bool> IsParticipantAsync(Guid ticketId, Guid userId);
        Task<List<TicketParticipant>> GetByTicketAsync(Guid ticketId);
        Task RemoveByTicketAndUserAsync(Guid ticketId, Guid userId);

    }
}
