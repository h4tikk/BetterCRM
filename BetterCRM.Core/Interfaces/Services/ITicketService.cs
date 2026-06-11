using BetterCRM.Core.Extensions;
using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record CreateTicketCommand(
        string Title, string? Description, TicketPriority Priority,
        Guid CreatorId, Guid? DepartmentId, Guid? AssigneeId);

    public record TransferTicketCommand(
        Guid TicketId,
        Guid TargetDepartmentId,
        Guid? TargetAssigneeId,
        string? Reason,
        Guid RequesterId);

    public interface ITicketService
    {
        Task<Ticket> CreateAsync(CreateTicketCommand cmd);
        Task<Ticket?> GetByIdAsync(Guid id);

        Task<List<Ticket>> GetFilteredAsync(Guid userId, string role, Guid? departmentId);

        Task ApproveAsync(Guid ticketId, Guid approverId);
        Task RejectAsync(Guid ticketId, Guid rejecterId);

        Task ResolveAsync(Guid ticketId, Guid resolverId);
        Task CloseAsync(Guid ticketId, Guid closerId);
        Task TransferAsync(TransferTicketCommand cmd);

        Task AddParticipantAsync(Guid ticketId, Guid userId, string role, Guid requesterId);
        Task RemoveParticipantAsync(Guid ticketId, Guid userId, Guid requesterId);
        Task<List<TicketParticipant>> GetParticipantsAsync(Guid ticketId);

        Task<int> CheckAndMarkOverdueAsync();
    }
}
