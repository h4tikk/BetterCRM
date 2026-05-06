using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class ShiftConfiguration : IEntityTypeConfiguration<ShiftEntity>
    {
        public void Configure(EntityTypeBuilder<ShiftEntity> builder)
        {
            builder.ToTable("shifts");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Status).IsRequired().HasMaxLength(12);
            builder.HasIndex(s => new { s.OrganizationId, s.UserId, s.Date }).IsUnique();
            builder.Property(s => s.LatenessPenaltyHours)
                   .HasColumnType("decimal(5,2)").HasDefaultValue(0m);
            builder.Property(s => s.EarlyLeavePenaltyHours)
                   .HasColumnType("decimal(5,2)").HasDefaultValue(0m);

            builder.HasOne<OrganizationEntity>()
                   .WithMany()
                   .HasForeignKey(s => s.OrganizationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.User)
                   .WithMany(u => u.Shifts)
                   .HasForeignKey(s => s.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.Date).HasColumnType("date");
            builder.Property(s => s.StartTime).HasColumnType("time");
            builder.Property(s => s.EndTime).HasColumnType("time");
            builder.Property(s => s.CreatedAt).HasColumnType("timestamptz");
            builder.Property(s => s.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}