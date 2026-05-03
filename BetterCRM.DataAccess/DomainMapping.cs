using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;

namespace BetterCRM.DataAccess
{
    public static class DomainMapper
    {
        public static Organization ToOrganizationDomain(OrganizationEntity db)
        {
            var result = Organization.Create(db.Name);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.org!;
            domain.Id = db.Id; domain.Slug = db.Slug; domain.IsActive = db.IsActive;
            domain.MainDirectorId = db.MainDirectorId; domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static OrganizationEntity ToOrganizationDb(Organization domain, OrganizationEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.Name = domain.Name; db.Slug = domain.Slug; db.IsActive = domain.IsActive;
            db.MainDirectorId = domain.MainDirectorId; db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static Department ToDepartmentDomain(DepartmentEntity db)
        {
            var result = Department.Create(db.OrganizationId, db.Name);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.department!;
            domain.Id = db.Id; domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static DepartmentEntity ToDepartmentDb(Department domain, DepartmentEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.Name = domain.Name;
            db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static Position ToPositionDomain(PositionEntity db)
        {
            var result = Position.Create(db.OrganizationId, db.Title, db.HourlyRate, db.DailyNormHours);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.position!;
            domain.Id = db.Id; domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static PositionEntity ToPositionDb(Position domain, PositionEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.Title = domain.Title;
            db.HourlyRate = domain.HourlyRate; db.DailyNormHours = domain.DailyNormHours;
            db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static User ToUserDomain(UserEntity db)
        {
            var result = User.Create(db.OrganizationId, db.Email, "MAPPED_HASH", db.FullName, db.Role, db.PositionId, db.DepartmentId, db.HireDate);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.user!;
            domain.Id = db.Id; domain.PasswordHash = db.PasswordHash; domain.IsActive = db.IsActive;
            domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static UserEntity ToUserDb(User domain, UserEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.Email = domain.Email;
            db.PasswordHash = domain.PasswordHash; db.FullName = domain.FullName; db.Role = domain.Role;
            db.PositionId = domain.PositionId; db.DepartmentId = (Guid)domain.DepartmentId; db.HireDate = domain.HireDate;
            db.IsActive = domain.IsActive; db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static Shift ToShiftDomain(ShiftEntity db)
        {
            var result = Shift.Create(db.OrganizationId, db.UserId, db.Date, db.StartTime, db.EndTime);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.shift!;
            domain.Id = db.Id; domain.Status = db.Status; domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static ShiftEntity ToShiftDb(Shift domain, ShiftEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.UserId = domain.UserId;
            db.Date = domain.Date; db.StartTime = domain.StartTime; db.EndTime = domain.EndTime;
            db.Status = domain.Status; db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static WorkSession ToWorkSessionDomain(WorkSessionEntity db)
        {
            var result = WorkSession.Start(db.OrganizationId, db.UserId, db.ShiftId);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.workSession!;
            domain.Id = db.Id; domain.StartedAt = db.StartedAt; domain.EndedAt = db.EndedAt; domain.Comment = db.Comment;
            domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static WorkSessionEntity ToWorkSessionDb(WorkSession domain, WorkSessionEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.UserId = domain.UserId;
            db.ShiftId = domain.ShiftId; db.StartedAt = domain.StartedAt; db.EndedAt = domain.EndedAt;
            db.Comment = domain.Comment; db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static TimeLog ToTimeLogDomain(TimeLogEntity db)
        {
            var result = TimeLog.Create(db.OrganizationId, db.WorkSessionId, db.TicketId, db.DurationHours, db.Description);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.log!;
            domain.Id = db.Id; domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static TimeLogEntity ToTimeLogDb(TimeLog domain, TimeLogEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.WorkSessionId = domain.WorkSessionId;
            db.TicketId = domain.TicketId; db.DurationHours = domain.DurationHours; db.Description = domain.Description;
            db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static Ticket ToTicketDomain(TicketEntity db)
        {
            var result = Ticket.Create(db.OrganizationId, db.Title, db.Description, db.Priority, db.CreatorId, db.AssigneeId, db.SLATargetHours);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.ticket!;
            domain.Id = db.Id; domain.Status = db.Status; domain.ResolvedAt = db.ResolvedAt;
            domain.IsSLABreached = db.IsSLABreached; domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static TicketEntity ToTicketDb(Ticket domain, TicketEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.Title = domain.Title;
            db.Description = domain.Description; db.Priority = domain.Priority; db.Status = domain.Status;
            db.CreatorId = domain.CreatorId; db.AssigneeId = domain.AssigneeId; db.ResolvedAt = domain.ResolvedAt;
            db.SLATargetHours = domain.SLATargetHours; db.IsSLABreached = domain.IsSLABreached;
            db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static TicketParticipant ToParticipantDomain(TicketParticipantEntity db)
        {
            var result = TicketParticipant.Create(db.OrganizationId, db.TicketId, db.UserId, db.Role);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.p!;
            domain.Id = db.Id; domain.JoinedAt = db.JoinedAt; domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static TicketParticipantEntity ToParticipantDb(TicketParticipant domain, TicketParticipantEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.TicketId = domain.TicketId;
            db.UserId = domain.UserId; db.Role = domain.Role; db.JoinedAt = domain.JoinedAt;
            db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }

        public static PayrollRecord ToPayrollDomain(PayrollRecordEntity db)
        {
            var result = PayrollRecord.Create(
                db.OrganizationId, db.UserId, db.PeriodStart, db.PeriodEnd,
                db.ScheduledHours, db.ActualHours, db.PenaltyHours, db.BillableHours, db.HourlyRate);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var domain = result.rec!;
            domain.Id = db.Id; domain.Status = db.Status; domain.CalculatedSalary = db.CalculatedSalary;
            domain.CreatedAt = db.CreatedAt; domain.UpdatedAt = db.UpdatedAt;
            return domain;
        }
        public static PayrollRecordEntity ToPayrollDb(PayrollRecord domain, PayrollRecordEntity? db = null)
        {
            db ??= new();
            db.Id = domain.Id; db.OrganizationId = domain.OrganizationId; db.UserId = domain.UserId;
            db.PeriodStart = domain.PeriodStart; db.PeriodEnd = domain.PeriodEnd;
            db.ScheduledHours = domain.ScheduledHours; db.ActualHours = domain.ActualHours;
            db.PenaltyHours = domain.PenaltyHours; db.BillableHours = domain.BillableHours;
            db.HourlyRate = domain.HourlyRate; db.CalculatedSalary = domain.CalculatedSalary;
            db.Status = domain.Status; db.CreatedAt = domain.CreatedAt; db.UpdatedAt = (DateTime)domain.UpdatedAt;
            return db;
        }
    }
}
