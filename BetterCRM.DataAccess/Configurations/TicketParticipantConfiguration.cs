using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class TicketParticipantConfiguration : IEntityTypeConfiguration<TicketParticipantEntity>
    {
        public void Configure(EntityTypeBuilder<TicketParticipantEntity> builder)
        {
            builder.ToTable("ticket_participants");
            builder.HasKey(tp => tp.Id);
            builder.Property(tp => tp.Role).IsRequired().HasMaxLength(30);
            builder.HasIndex(tp => new { tp.OrganizationId, tp.TicketId, tp.UserId }).IsUnique();

            builder.HasOne<OrganizationEntity>().WithMany().HasForeignKey(tp => tp.OrganizationId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(tp => tp.Ticket).WithMany(t => t.Participants).HasForeignKey(tp => tp.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(tp => tp.User).WithMany().HasForeignKey(tp => tp.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(tp => tp.JoinedAt).HasColumnType("timestamptz");
            builder.Property(tp => tp.CreatedAt).HasColumnType("timestamptz");
        }
    }
}
