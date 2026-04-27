using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record CreateTicketCommand(string Title, string? Description, string Priority, Guid CreatorId, Guid? AssigneeId);
    public record UpdateTicketCommand(string? Title, string? Description, string? Priority, string? status, Guid? AssigneeId);
    public interface ITicketService
    {
        Task<Ticket> CreateAsync(CreateTicketCommand command);
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<List<Ticket>> GetAllForUserAsync(Guid userId, string role, Guid? departmentId);
        Task<Ticket> UpdateAsync(Guid id, UpdateTicketCommand command);
        Task DeleteAsync(Guid id);
        Task ResolveTicketAsync(Guid ticketId);
        Task CloseTicketAsync(Guid ticketId);
        Task ReopenTicketAsync(Guid ticketId);
        Task AssignTicketAsync(Guid ticketId, Guid? assigneeId);
        Task AddParticipantAsync(Guid ticketId, Guid userId, string role);
        Task RemoveParticipantAsync(Guid ticketId, Guid userId);
        Task<List<TicketParticipant>> GetParticipantsAsync(Guid ticketId);
        Task<int> CheckAndMarkOverdueAsync();
        Task<List<Ticket>> GetOverdueTicketsAsync(Guid? departmentId = null);
        Task<List<Ticket>> SearchAsync(string searchTerm, Guid? departmentId = null);
    }
}
