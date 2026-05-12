using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketCommentEntity>
    {
        public void Configure(EntityTypeBuilder<TicketCommentEntity> builder)
        {
            builder.ToTable("ticket_comments");

            builder.Property(c => c.Text)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(c => c.UpdatedAt)
                .HasColumnType("timestamptz");

            builder.HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Attachments)
                .WithOne(a => a.Comment)
                .HasForeignKey(a => a.CommentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => new { c.TicketId, c.CreatedAt })
                .HasDatabaseName("IX_ticket_comments_ticket");
        }
    }
}
