using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public interface ITicketCommentService
    {
        Task<List<TicketComment>> GetByTicketAsync(Guid ticketId);
        Task<(TicketComment? comment, string? error)> AddCommentAsync(
            Guid ticketId, Guid authorId, string authorName,
            Guid organizationId, string text,
            IEnumerable<(Stream stream, string fileName, string contentType, long size)>? files = null);
        Task<(bool success, string? error)> UpdateCommentAsync(
            Guid commentId, Guid requesterId, string newText);
        Task<(bool success, string? error)> DeleteCommentAsync(
            Guid commentId, Guid requesterId, string requesterRole);
        Task<(TicketAttachment? attachment, string? error)> AddAttachmentAsync(
            Guid ticketId, Guid commentId, Guid uploaderId,
            Guid organizationId, Stream stream,
            string fileName, string contentType, long size);
        Task<(bool success, string? error)> DeleteAttachmentAsync(
            Guid attachmentId, Guid requesterId, string requesterRole);
    }
}
