using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class ShiftBreakConfiguration : IEntityTypeConfiguration<ShiftBreakEntity>
    {
        public void Configure(EntityTypeBuilder<ShiftBreakEntity> builder)
        {
            builder.ToTable("shift_breaks");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Type).IsRequired().HasMaxLength(12);
            builder.Property(b => b.StartTime).HasColumnType("time");
            builder.Property(b => b.EndTime).HasColumnType("time");
            builder.HasIndex(b => new { b.OrganizationId, b.ShiftId });

            builder.HasOne<OrganizationEntity>()
                   .WithMany()
                   .HasForeignKey(b => b.OrganizationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Shift)
                   .WithMany(s => s.Breaks)
                   .HasForeignKey(b => b.ShiftId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(b => b.CreatedAt).HasColumnType("timestamptz");
            builder.Property(b => b.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}
