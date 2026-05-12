using BetterCRM.Core.Extensions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Messages;
using BetterCRM.Core.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BetterCRM.Business.Services
{
    public class TicketCommentService : ITicketCommentService
    {
        private readonly ITicketCommentRepository _commentRepo;
        private readonly ITicketRepository _ticketRepo;
        private readonly IFileStorageService _storage;
        private readonly IPublishEndpoint _bus;
        private readonly ILogger<TicketCommentService> _logger;

        private const string Bucket = "ticket-attachments";

        public TicketCommentService(
            ITicketCommentRepository commentRepo,
            ITicketRepository ticketRepo,
            IFileStorageService storage,
            IPublishEndpoint bus,
            ILogger<TicketCommentService> logger)
        {
            _commentRepo = commentRepo;
            _ticketRepo = ticketRepo;
            _storage = storage;
            _bus = bus;
            _logger = logger;
        }
        public async Task<(TicketAttachment? attachment, string? error)> AddAttachmentAsync(Guid ticketId, Guid commentId, Guid uploaderId, Guid organizationId, Stream stream, string fileName, string contentType, long size)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId);
            if (comment is null) return (null, "Комментарий не найден");
            if (comment.AuthorId != uploaderId) return (null, "Нет прав");

            var objectName = $"{ticketId}/{commentId}/{Guid.NewGuid()}-{fileName}";
            await _storage.UploadAsync(Bucket, objectName, stream, contentType);

            var attachment = TicketAttachment.Restore(
                id: Guid.NewGuid(),
                organizationId: organizationId,
                ticketId: ticketId,
                commentId: commentId,
                uploaderId: uploaderId,
                fileName: fileName,
                objectName: objectName,
                contentType: contentType,
                sizeBytes: size,
                createdAt: DateTime.UtcNow
            );

            await _commentRepo.AddAttachmentAsync(attachment);
            return (attachment, null);
        }

        public async Task<(TicketComment? comment, string? error)> AddCommentAsync(Guid ticketId, Guid authorId, string authorName, Guid organizationId, string text, IEnumerable<(Stream stream, string fileName, string contentType, long size)>? files = null)
        {
            var ticket = await _ticketRepo.GetByIdAsync(ticketId);
            if (ticket is null)
                return (null, "Тикет не найден");

            if (ticket.Status is TicketStatus.Closed)
                return (null, "Нельзя комментировать закрытый тикет");

            var (comment, error) = TicketComment.Create(
                organizationId, ticketId, authorId, authorName, text);

            if (comment is null) return (null, error);

            await _commentRepo.AddAsync(comment);

            if (files is not null)
            {
                foreach (var (stream, fileName, contentType, size) in files)
                {
                    var objectName = $"{ticketId}/{comment.Id}/{Guid.NewGuid()}-{fileName}";
                    await _storage.UploadAsync(Bucket, objectName, stream, contentType);

                    var attachment = TicketAttachment.Restore(
                        id: Guid.NewGuid(),
                        organizationId: organizationId,
                        ticketId: ticketId,
                        commentId: comment.Id,
                        uploaderId: authorId,
                        fileName: fileName,
                        objectName: objectName,
                        contentType: contentType,
                        sizeBytes: size,
                        createdAt: DateTime.UtcNow
                    );

                    await _commentRepo.AddAttachmentAsync(attachment);
                }
            }

            await _bus.Publish(new TicketNotificationEvent(
                TicketId: ticketId,
                OrganizationId: organizationId,
                TicketTitle: ticket.Title,
                Type: NotifyType.CommentAdded,
                TriggeredByUserId: authorId,
                TriggeredByName: authorName,
                AssigneeId: ticket.AssigneeId,
                DepartmentId: ticket.DepartmentId ?? Guid.Empty,
                CommentText: text,
                OccurredAt: DateTime.UtcNow
            ));

            return (comment, null);
        }

        public async Task<(bool success, string? error)> DeleteAttachmentAsync(Guid attachmentId, Guid requesterId, string requesterRole)
        {
            var attachment = await _commentRepo.GetAttachmentAsync(attachmentId);
            if (attachment is null) return (false, "Вложение не найдено");

            var isOwner = attachment.UploaderId == requesterId;
            var isAdmin = requesterRole is "Admin" or "DepartmentHead";

            if (!isOwner && !isAdmin)
                return (false, "Нет прав на удаление");

            await _storage.DeleteAsync(Bucket, attachment.ObjectName);
            await _commentRepo.DeleteAttachmentAsync(attachmentId);
            return (true, null);
        }

        public async Task<(bool success, string? error)> DeleteCommentAsync(Guid commentId, Guid requesterId, string requesterRole)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId);
            if (comment is null) return (false, "Комментарий не найден");

            var isOwner = comment.AuthorId == requesterId;
            var isAdmin = requesterRole is "Admin" or "DepartmentHead";

            if (!isOwner && !isAdmin)
                return (false, "Нет прав на удаление");

            foreach (var attachment in comment.Attachments)
            {
                await _storage.DeleteAsync(Bucket, attachment.ObjectName);
            }

            await _commentRepo.DeleteAsync(commentId);
            return (true, null);
        }

        public async Task<List<TicketComment>> GetByTicketAsync(Guid ticketId) =>
            await _commentRepo.GetByTicketAsync(ticketId);

        public async Task<(bool success, string? error)> UpdateCommentAsync(Guid commentId, Guid requesterId, string newText)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId);
            if (comment == null) return (false, "Комментарий не найден");
            if (comment.AuthorId != requesterId) return (false, "Нет прав на редактирование");
            try
            {
                comment.Update(newText);
                await _commentRepo.UpdateAsync(comment);
                return (true, null);
            }
            catch(InvalidOperationException ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
