using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BetterCRM.DataAccess.Repositories
{
    public class TicketCommentRepository : ITicketCommentRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string _minioBase;

        public TicketCommentRepository(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _minioBase = $"http://{config["Minio:Endpoint"]}";
        }

        public async Task<List<TicketComment>> GetByTicketAsync(Guid ticketId) =>
            (await _context.TicketComments
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .Include(c => c.Author)
                .Include(c => c.Attachments)
                .AsNoTracking()
                .ToListAsync())
                .Select(c => DomainMapper.ToTicketCommentDomain(c, _minioBase))
                .ToList();

        public async Task<TicketComment?> GetByIdAsync(Guid commentId)
        {
            var entity = await _context.TicketComments
                .Include(c => c.Author)
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(c => c.Id == commentId);

            return entity is null ? null : DomainMapper.ToTicketCommentDomain(entity, _minioBase);
        }

        public async Task AddAsync(TicketComment comment)
        {
            await _context.TicketComments.AddAsync(DomainMapper.ToTicketCommentDb(comment));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TicketComment comment)
        {
            var entity = await _context.TicketComments.FindAsync(comment.Id);
            if (entity is null) return;

            entity.Text = comment.Text;
            entity.UpdatedAt = comment.UpdatedAt ?? comment.CreatedAt;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid commentId)
        {
            await _context.TicketComments
                .Where(c => c.Id == commentId)
                .ExecuteDeleteAsync();
        }

        public async Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment)
        {
            var entity = DomainMapper.ToTicketAttachmentDb(attachment);
            await _context.TicketAttachments.AddAsync(entity);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<TicketAttachment?> GetAttachmentAsync(Guid attachmentId)
        {
            var entity = await _context.TicketAttachments
                .FindAsync(attachmentId);
            return entity is null ? null : DomainMapper.ToTicketAttachmentDomain(entity, _minioBase);
        }

        public async Task DeleteAttachmentAsync(Guid attachmentId)
        {
            await _context.TicketAttachments
                .Where(a => a.Id == attachmentId)
                .ExecuteDeleteAsync();
        }
    }
}
