using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");
            builder.HasIndex(t => t.Id);

            builder.Property(t => t.Title).IsRequired().HasMaxLength(Ticket.MaxTitleLength);
            builder.Property(t => t.Description).HasMaxLength(Ticket.MaxDescriptionLength);
            builder.Property(t => t.Priority).IsRequired().HasMaxLength(20);
            builder.Property(t => t.Status).IsRequired().HasMaxLength(30);
            builder.Property(t => t.SLATargetHours).HasColumnType("decimal(5,2)");

            builder.HasIndex(t => new { t.Status, t.CreatedAt });
            builder.HasIndex(t => t.AssigneeId);
            builder.HasIndex(t => t.CreatorId);

            builder.HasOne(t => t.Creator)
                .WithMany(u => u.CreatedTickets)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Assignee)
                .WithMany(t => t.AssignedTickets)
                .HasForeignKey(t => t.AssigneeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(t => t.CreatedAt).HasColumnType("timestampz");
            builder.Property(t => t.ResolvedAt).HasColumnType("timestampz");
            builder.Property(t => t.UpdatedAt).HasColumnType("timestampz");
        }
    }
}
