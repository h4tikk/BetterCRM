using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BetterCRM.Core.Models;

namespace BetterCRM.DataAccess.Configurations
{
    public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
    {
        public void Configure(EntityTypeBuilder<PayrollRecord> builder)
        {
            builder.ToTable("payroll_records");
            builder.HasKey(pr => pr.Id);
            builder.Property(pr => pr.ScheduledHours).HasColumnType("decimal(6,2)");
            builder.Property(pr => pr.ActualHours).HasColumnType("decimal(6,2)");
            builder.Property(pr => pr.PenaltyHours).HasColumnType("decimal(6,2)");
            builder.Property(pr => pr.BillableHours).HasColumnType("decimal(6,2)");
            builder.Property(pr => pr.HourlyRate).HasColumnType("decimal(10,2)");
            builder.Property(pr => pr.CalculatedSalary).HasColumnType("decimal(10,2)");
            builder.Property(pr => pr.Status).IsRequired().HasMaxLength(20);

            builder.HasIndex(pr => new { pr.OrganizationId, pr.UserId, pr.PeriodStart }).IsUnique();

            builder.HasOne<Organization>().WithMany().HasForeignKey(pr => pr.OrganizationId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(pr => pr.User).WithMany(u => u.PayrollRecords).HasForeignKey(pr => pr.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(pr => pr.PeriodStart).HasColumnType("date");
            builder.Property(pr => pr.PeriodEnd).HasColumnType("date");
            builder.Property(pr => pr.CreatedAt).HasColumnType("timestamptz");
        }
    }
}
