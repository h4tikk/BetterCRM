using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.DataAccess.Entities;

namespace BetterCRM.DataAccess.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<DepartmentEntity>
    {
        public void Configure(EntityTypeBuilder<DepartmentEntity> builder) 
        {
            builder.ToTable("departments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(d => new { d.OrganizationId, d.Name }).IsUnique();

            builder.HasOne(d => d.Organization)
                .WithMany(o => o.Departments)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(d => d.CreatedAt).HasColumnType("timestamptz");
            builder.Property(d => d.UpdatedAt).HasColumnType("timestamptz");
        }
    }
}
