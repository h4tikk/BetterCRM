using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder) 
        {
            builder.ToTable("departments");
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(Department.MaxNameLength);
            builder.HasIndex(d => new { d.OrganizationId, d.Name }).IsUnique();

            builder.HasOne<Organization>()
                .WithMany()
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(d => d.CreatedAt).HasColumnType("timestampz");
            builder.Property(d => d.UpdatedAt).HasColumnType("timestampz");
        }
    }
}
