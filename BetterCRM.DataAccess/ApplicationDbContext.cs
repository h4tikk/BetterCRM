using Microsoft.EntityFrameworkCore;
using BetterCRM.Core.Models;
using System.Linq.Expressions;

namespace BetterCRM.DataAccess
{
    internal class ApplicationDbContext : DbContext
    {
        public Guid CurrentOrganizationId { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Shift> Shifts => Set<Shift>();
        public DbSet<WorkSession> WorkSessions => Set<WorkSession>();
        public DbSet<TimeLog> TimeLogs => Set<TimeLog>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketParticipant> TicketParticipants => Set<TicketParticipant>();
        public DbSet<PayrollRecord> PayrollRecords => Set<PayrollRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            var tenantEntity = typeof(TenantEntity);
            foreach(var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (tenantEntity.IsAssignableFrom(entity.ClrType) && !entity.ClrType.IsAbstract)
                {
                    var param = Expression.Parameter(entity.ClrType, "e");
                    var orgIdProp = Expression.Property(param, nameof(TenantEntity.OrganizationId));
                    var dbContextOrgId = Expression.Property(Expression.Constant(this), nameof(CurrentOrganizationId));
                    var condition = Expression.Equal(orgIdProp, dbContextOrgId);
                    var lambda = Expression.Lambda(condition, param);
                    entity.SetQueryFilter(lambda);
                }
            }
        }

    }
}
