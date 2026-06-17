using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BetterCRM.DataAccess.Configurations
{
    internal class OrganizationConfiguration : IEntityTypeConfiguration<OrganizationEntity>
    {
        public void Configure(EntityTypeBuilder<OrganizationEntity> builder)
        {
            builder.ToTable("organizations");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name).IsRequired().HasMaxLength(Organization.MaxNameLength);
            builder.Property(o => o.Slug).IsRequired().HasMaxLength(100);
            builder.HasIndex(o => o.Slug).IsUnique();

            builder.HasOne(o => o.MainDirector)
                .WithOne()
                .HasForeignKey<OrganizationEntity>(o => o.MainDirectorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(o => o.CreatedAt).HasColumnType("timestamptz");
            builder.Property(o => o.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}