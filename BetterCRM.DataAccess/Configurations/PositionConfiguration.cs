using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("positions");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(Position.MaxTitleLength);

            builder.Property(p => p.HourlyRate)
                .IsRequired()
                .HasColumnType("decimal(10,2)");
            builder.Property(p => p.Da)

            builder.HasMany(p => p.Users)
                .WithOne(u => u.Position)
                .HasForeignKey(u => u.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
