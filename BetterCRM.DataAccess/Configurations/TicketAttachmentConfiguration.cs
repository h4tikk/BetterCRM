using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachmentEntity>
    {
        public void Configure(EntityTypeBuilder<TicketAttachmentEntity> builder)
        {
            builder.ToTable("ticket_attachments");

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.ObjectName)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(a => a.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Uploader)
                .WithMany()
                .HasForeignKey(a => a.UploaderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(a => new { a.TicketId, a.CommentId })
                .HasDatabaseName("IX_ticket_attachments_ticket");
        }
    }
}
