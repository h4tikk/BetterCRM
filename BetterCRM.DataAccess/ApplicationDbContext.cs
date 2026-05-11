using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BetterCRM.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public Guid CurrentOrganizationId { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<OrganizationEntity> Organizations => Set<OrganizationEntity>();
        public DbSet<DepartmentEntity> Departments => Set<DepartmentEntity>();
        public DbSet<PositionEntity> Positions => Set<PositionEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ShiftEntity> Shifts => Set<ShiftEntity>();
        public DbSet<WorkSessionEntity> WorkSessions => Set<WorkSessionEntity>();
        public DbSet<TimeLogEntity> TimeLogs => Set<TimeLogEntity>();
        public DbSet<TicketEntity> Tickets => Set<TicketEntity>();
        public DbSet<TicketParticipantEntity> TicketParticipants => Set<TicketParticipantEntity>();
        public DbSet<PayrollRecordEntity> PayrollRecords => Set<PayrollRecordEntity>();
        public DbSet<ChatMessageEntity> ChatMessages => Set<ChatMessageEntity>();
        public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            var tenantType = typeof(TenantDbEntity);
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (tenantType.IsAssignableFrom(entity.ClrType) && !entity.ClrType.IsAbstract)
                {
                    var param = Expression.Parameter(entity.ClrType, "e");
                    var orgIdProp = Expression.Property(param, nameof(TenantDbEntity.OrganizationId));
                    var dbContextOrgId = Expression.Property(Expression.Constant(this), nameof(CurrentOrganizationId));
                    var condition = Expression.Equal(orgIdProp, dbContextOrgId);
                    var lambda = Expression.Lambda(condition, param);
                    entity.SetQueryFilter(lambda);
                }
            }
        }
    }
}
