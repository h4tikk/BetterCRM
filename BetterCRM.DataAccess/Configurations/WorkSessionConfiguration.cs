using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.Core.Models;
namespace BetterCRM.DataAccess.Configurations
{
    public class WorkSessionConfiguration : IEntityTypeConfiguration<WorkSession>
    {
        public void Configure(EntityTypeBuilder<WorkSession> builder)
        {
            builder.ToTable("work_sessions");
            builder.HasKey(ws => ws.Id);

            builder.Property(ws => ws.Description).HasMaxLength(WorkSession.MaxDescriptionLength);
            builder.HasIndex(ws => new { ws.UserId, ws.StartedAt });
            builder.HasIndex(ws => ws.TicketId);

            builder.HasOne(ws => ws.User)
                .WithMany(u => u.WorkSessions)
                .HasForeignKey(ws => ws.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ws => ws.Ticket)
                .WithMany(t => t.WorkSessions)
                .HasForeignKey(ws => ws.TicketId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(ws => ws.StartedAt).HasColumnType("timestamptz");
            builder.Property(ws => ws.EndedAt).HasColumnType("timestamptz");
            builder.Property(ws => ws.CreatedAt).HasColumnType("timestamptz");
            builder.Property(ws => ws.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}
