using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record CreateTicketCommand(string Title, string? Description, string Priority, Guid CreatorId, Guid? AssigneeId);
    public record UpdateTicketCommand(string? Title, string? Description, string? Priority, string? status, Guid? AssigneeId);
    public interface ITicketService
    {
        Task<Ticket> CreateAsync(CreateTicketCommand cmd);
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<List<Ticket>> GetFilteredAsync(Guid userId, string role, Guid? departmentId);
        Task ResolveAsync(Guid ticketId);
        Task AddParticipantAsync(Guid ticketId, Guid userId, string role);
        Task RemoveParticipantAsync(Guid ticketId, Guid userId);
        Task<List<TicketParticipant>> GetParticipantsAsync(Guid ticketId);
        Task<int> CheckAndMarkOverdueAsync();
    }
}
