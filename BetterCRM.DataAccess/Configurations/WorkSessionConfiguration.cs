using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BetterCRM.DataAccess.Configurations
{
    public class WorkSessionConfiguration : IEntityTypeConfiguration<WorkSessionEntity>
    {
        public void Configure(EntityTypeBuilder<WorkSessionEntity> builder)
        {
            builder.ToTable("work_sessions");
            builder.HasKey(ws => ws.Id);
            builder.Property(ws => ws.Comment).HasMaxLength(WorkSession.MaxCommentLength);
            builder.HasIndex(ws => new { ws.OrganizationId, ws.UserId, ws.StartedAt });

            builder.HasOne<OrganizationEntity>()
                .WithMany()
                .HasForeignKey(ws => ws.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ws => ws.User)
                .WithMany(u => u.WorkSessions)
                .HasForeignKey(ws => ws.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ws => ws.Shift)
                .WithMany()
                .HasForeignKey(ws => ws.ShiftId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(ws => ws.TimeLogs)
                .WithOne(tl => tl.WorkSession)
                .HasForeignKey(tl => tl.WorkSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ws => ws.StartedAt).HasColumnType("timestamptz");
            builder.Property(ws => ws.EndedAt).HasColumnType("timestamptz");
            builder.Property(ws => ws.CreatedAt).HasColumnType("timestamptz");
            builder.Property(ws => ws.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}
