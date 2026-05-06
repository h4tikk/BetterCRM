using BetterCRM.Core.Extensions;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;

namespace BetterCRM.DataAccess
{
    public static class DomainMapper
    {
        // ── Organization ──────────────────────────────────────────────────────
        public static Organization ToOrganizationDomain(OrganizationEntity db)
        {
            var result = Organization.Create(db.Name);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.org!;
            d.Id = db.Id; d.Slug = db.Slug; d.IsActive = db.IsActive;
            d.MainDirectorId = db.MainDirectorId;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static OrganizationEntity ToOrganizationDb(Organization d, OrganizationEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.Name = d.Name; db.Slug = d.Slug; db.IsActive = d.IsActive;
            db.MainDirectorId = d.MainDirectorId;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        // ── Department ────────────────────────────────────────────────────────
        public static Department ToDepartmentDomain(DepartmentEntity db)
        {
            var result = Department.Create(db.OrganizationId, db.Name);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.department!;
            d.Id = db.Id;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static DepartmentEntity ToDepartmentDb(Department d, DepartmentEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.Name = d.Name;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        // ── Position ──────────────────────────────────────────────────────────
        public static Position ToPositionDomain(PositionEntity db)
        {
            var result = Position.Create(db.OrganizationId, db.Title, db.HourlyRate, db.DailyNormHours);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.position!;
            d.Id = db.Id;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static PositionEntity ToPositionDb(Position d, PositionEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.Title = d.Title;
            db.HourlyRate = d.HourlyRate; db.DailyNormHours = d.DailyNormHours;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        // ── User ──────────────────────────────────────────────────────────────
        public static User ToUserDomain(UserEntity db)
        {
            var result = User.Create(db.OrganizationId, db.Email, "MAPPED_HASH",
                db.FullName, db.Role, db.PositionId, db.DepartmentId, db.HireDate);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.user!;
            d.Id = db.Id;
            // ✅ ИСПРАВЛЕНО: восстанавливаем реальный хэш, а не "MAPPED_HASH"
            d.PasswordHash = db.PasswordHash;
            d.IsActive = db.IsActive;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static UserEntity ToUserDb(User d, UserEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.Email = d.Email;
            db.PasswordHash = d.PasswordHash; db.FullName = d.FullName; db.Role = d.Role;
            db.PositionId = d.PositionId; db.DepartmentId = (Guid)d.DepartmentId;
            db.HireDate = d.HireDate; db.IsActive = d.IsActive;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        public static Shift ToShiftDomain(ShiftEntity db)
        {
            var result = Shift.Create(db.OrganizationId, db.UserId, db.Date, db.StartTime, db.EndTime);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.shift!;
            d.Id = db.Id;
            d.Status = Enum.Parse<ShiftStatus>(db.Status);
            d.LatenessPenaltyHours = db.LatenessPenaltyHours;
            d.EarlyLeavePenaltyHours = db.EarlyLeavePenaltyHours;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static ShiftEntity ToShiftDb(Shift d, ShiftEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.UserId = d.UserId;
            db.Date = d.Date; db.StartTime = d.StartTime; db.EndTime = d.EndTime;
            db.Status = d.Status.ToString();
            db.LatenessPenaltyHours = d.LatenessPenaltyHours;
            db.EarlyLeavePenaltyHours = d.EarlyLeavePenaltyHours;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        // ── WorkSession ───────────────────────────────────────────────────────
        public static WorkSession ToWorkSessionDomain(WorkSessionEntity db)
        {
            var result = WorkSession.Start(db.OrganizationId, db.UserId, db.ShiftId);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.workSession!;
            d.Id = db.Id;
            d.StartedAt = db.StartedAt; d.EndedAt = db.EndedAt; d.Comment = db.Comment;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static WorkSessionEntity ToWorkSessionDb(WorkSession d, WorkSessionEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.UserId = d.UserId;
            db.ShiftId = d.ShiftId; db.StartedAt = d.StartedAt; db.EndedAt = d.EndedAt;
            db.Comment = d.Comment;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        // ── TimeLog ───────────────────────────────────────────────────────────
        public static TimeLog ToTimeLogDomain(TimeLogEntity db)
        {
            var result = TimeLog.Create(db.OrganizationId, db.WorkSessionId, db.TicketId,
                db.DurationHours, db.Description);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.log!;
            d.Id = db.Id;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static TimeLogEntity ToTimeLogDb(TimeLog d, TimeLogEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.WorkSessionId = d.WorkSessionId;
            db.TicketId = d.TicketId; db.DurationHours = d.DurationHours; db.Description = d.Description;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        // ── Ticket ────────────────────────────────────────────────────────────
        // ✅ ИЗМЕНЕНО: Priority и Status маппятся через Enum.Parse / ToString
        // ✅ ИЗМЕНЕНО: добавлены DepartmentId, ClosedAt, OverduePenaltyHours
        public static Ticket ToTicketDomain(TicketEntity db)
        {
            var result = Ticket.Create(
                db.OrganizationId, db.Title, db.Description,
                Enum.Parse<TicketPriority>(db.Priority),
                db.CreatorId, db.DepartmentId,
                db.AssigneeId, db.SLATargetHours);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.ticket!;
            d.Id = db.Id;
            d.Status = Enum.Parse<TicketStatus>(db.Status);
            d.ResolvedAt = db.ResolvedAt;
            d.ClosedAt = db.ClosedAt;
            d.IsSLABreached = db.IsSLABreached;
            d.OverduePenaltyHours = db.OverduePenaltyHours;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static TicketEntity ToTicketDb(Ticket d, TicketEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId;
            db.Title = d.Title; db.Description = d.Description;
            db.Priority = d.Priority.ToString();
            db.Status = d.Status.ToString();
            db.CreatorId = d.CreatorId; db.AssigneeId = d.AssigneeId;
            db.DepartmentId = d.DepartmentId;
            db.ResolvedAt = d.ResolvedAt; db.ClosedAt = d.ClosedAt;
            db.SLATargetHours = d.SLATargetHours;
            db.IsSLABreached = d.IsSLABreached;
            db.OverduePenaltyHours = d.OverduePenaltyHours;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }

        // ── TicketParticipant ─────────────────────────────────────────────────
        public static TicketParticipant ToParticipantDomain(TicketParticipantEntity db)
        {
            var result = TicketParticipant.Create(db.OrganizationId, db.TicketId, db.UserId, db.Role);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.p!;
            d.Id = db.Id; d.JoinedAt = db.JoinedAt;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static TicketParticipantEntity ToParticipantDb(TicketParticipant d, TicketParticipantEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId;
            db.TicketId = d.TicketId; db.UserId = d.UserId;
            db.Role = d.Role; db.JoinedAt = d.JoinedAt;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }


        public static PayrollRecord ToPayrollDomain(PayrollRecordEntity db)
        {
            var totalPenalty = db.AttendancePenaltyHours + db.TicketPenaltyHours;
            var billable = db.FinalBillableHours > 0
                ? db.FinalBillableHours
                : Math.Max(0, db.ActualHours - totalPenalty);

            var result = PayrollRecord.Create(
                db.OrganizationId, db.UserId,
                db.PeriodStart, db.PeriodEnd,
                db.ScheduledHours, db.ActualHours,
                totalPenalty,
                billable,
                db.HourlyRate);

            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.rec!;
            d.Id = db.Id;

            d.Status = db.Status.ToEnum<PayrollStatus>();

            d.CalculatedSalary = db.CalculatedSalary;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static PayrollRecordEntity ToPayrollDb(PayrollRecord d, PayrollRecordEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.UserId = d.UserId;
            db.PeriodStart = d.PeriodStart; db.PeriodEnd = d.PeriodEnd;
            db.ScheduledHours = d.ScheduledHours; db.ActualHours = d.ActualHours;
            db.AttendancePenaltyHours = d.AttendancePenaltyHours;
            db.TicketPenaltyHours = d.TicketPenaltyHours;
            db.TotalPenaltyHours = d.TotalPenaltyHours;
            db.FinalBillableHours = d.FinalBillableHours;
            db.HourlyRate = d.HourlyRate; db.CalculatedSalary = d.CalculatedSalary;
            db.Status = d.Status.ToString();
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = (DateTime)d.UpdatedAt;
            return db;
        }
    }
}