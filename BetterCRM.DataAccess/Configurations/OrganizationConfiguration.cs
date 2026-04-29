using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.Core.Models;
namespace BetterCRM.DataAccess.Configurations
{
    internal class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder) 
        {
            builder.ToTable("organizations");
            builder.HasIndex(o => o.Id);

            builder.Property(o => o.Name).IsRequired().HasMaxLength(Organization.MaxNameLength);
            builder.Property(o => o.Slug).IsRequired().HasMaxLength(100);
            builder.HasIndex(o => o.Slug).IsUnique();
            builder.Property(o => o.CreatedAt).HasColumnType("timestampz");
            builder.Property(o => o.UpdatedAt).HasColumnType("timestampz");
        }
    }
}
