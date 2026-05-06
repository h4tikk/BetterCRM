using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<TicketEntity>
    {
        public void Configure(EntityTypeBuilder<TicketEntity> builder)
        {
            builder.ToTable("tickets");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title).IsRequired().HasMaxLength(Ticket.MaxTitleLength);
            builder.Property(t => t.Description).HasMaxLength(Ticket.MaxDescriptionLength);
            builder.Property(t => t.Priority).IsRequired().HasMaxLength(10);
            builder.Property(t => t.Status).IsRequired().HasMaxLength(20);
            builder.Property(t => t.SLATargetHours).HasColumnType("decimal(5,2)");

            builder.Property(t => t.DepartmentId);
            builder.Property(t => t.ClosedAt).HasColumnType("timestamptz");
            builder.Property(t => t.OverduePenaltyHours)
                   .HasColumnType("decimal(5,2)").HasDefaultValue(0m);

            builder.HasIndex(t => new { t.OrganizationId, t.Status, t.CreatedAt });
            builder.HasIndex(t => new { t.OrganizationId, t.DepartmentId });

            builder.HasOne<OrganizationEntity>()
                   .WithMany()
                   .HasForeignKey(t => t.OrganizationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Creator)
                   .WithMany(u => u.CreatedTickets)
                   .HasForeignKey(t => t.CreatorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Assignee)
                   .WithMany(u => u.AssignedTickets)
                   .HasForeignKey(t => t.AssigneeId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.Participants)
                   .WithOne(tp => tp.Ticket)
                   .HasForeignKey(tp => tp.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.CreatedAt).HasColumnType("timestamptz");
            builder.Property(t => t.ResolvedAt).HasColumnType("timestamptz");
            builder.Property(t => t.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}