using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class TimeLogConfiguration : IEntityTypeConfiguration<TimeLogEntity>
    {
        public void Configure(EntityTypeBuilder<TimeLogEntity> builder) 
        {
            builder.ToTable("time_logs");
            builder.HasKey(tl => tl.Id);
            builder.Property(tl => tl.DurationHours).HasColumnType("decimal(5,2)");
            builder.HasIndex(tl => new { tl.OrganizationId, tl.TicketId });

            builder.HasOne(tl => tl.Organization)
                .WithMany(o => o.TimeLogs)
                .HasForeignKey(tl => tl.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tl => tl.WorkSession)
                .WithMany(ws => ws.TimeLogs)
                .HasForeignKey(tl => tl.WorkSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tl => tl.Ticket)
                .WithMany(t => t.TimeLogs)
                .HasForeignKey(tl => tl.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(tl => tl.CreatedAt).HasColumnType("timestamptz");


        }
    }
}
