using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Configurations
{
    public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.ToTable("shifts");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Status).IsRequired().HasMaxLength(12);
            builder.HasIndex(s => new { s.OrganizationId, s.UserId, s.Date }).IsUnique();

            builder.HasOne<Organization>().WithMany().HasForeignKey(s => s.OrganizationId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(s => s.User).WithMany(u => u.Shifts).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.Date).HasColumnType("date");
            builder.Property(s => s.StartTime).HasColumnType("time");
            builder.Property(s => s.EndTime).HasColumnType("time");
            builder.Property(s => s.CreatedAt).HasColumnType("timestamptz");


        }
    }
}
