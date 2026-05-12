using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface ITicketCommentRepository
    {
        Task<List<TicketComment>> GetByTicketAsync(Guid ticketId);
        Task<TicketComment?> GetByIdAsync(Guid commentId);
        Task AddAsync(TicketComment comment);
        Task UpdateAsync(TicketComment comment);
        Task DeleteAsync(Guid commentId);
        Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment);
        Task<TicketAttachment?> GetAttachmentAsync(Guid attachmentId);
        Task DeleteAttachmentAsync(Guid attachmentId);
    }
}
