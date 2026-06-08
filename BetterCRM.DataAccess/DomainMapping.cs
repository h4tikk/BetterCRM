using BetterCRM.Core.Extensions;
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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

        public static Position ToPositionDomain(PositionEntity db)
        {
            var result = Position.Create(db.OrganizationId, db.Title, db.HourlyRate, db.DailyNormHours, db.DepartmentId);
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
            db.DepartmentId = d.DepartmentId;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

        public static User ToUserDomain(UserEntity db)
        {
            var result = User.Create(db.OrganizationId, db.Email, "MAPPED_HASH",
                db.FullName, db.Role, db.PositionId, db.DepartmentId, db.HireDate);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.user!;
            d.Id = db.Id;
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
            db.PositionId = d.PositionId; db.DepartmentId = d.DepartmentId;
            db.HireDate = d.HireDate; db.IsActive = d.IsActive;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
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
            if (db.Breaks != null)
                foreach (var b in db.Breaks.OrderBy(b => b.StartTime))
                    d.Breaks.Add(ToShiftBreakDomain(b));
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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

        public static ShiftBreak ToShiftBreakDomain(ShiftBreakEntity db)
        {
            var result = ShiftBreak.Create(db.OrganizationId, db.ShiftId,
                db.StartTime, db.EndTime, Enum.Parse<BreakType>(db.Type), db.IsPaid);
            if (result.error != null) throw new InvalidOperationException($"DB Data Corrupted: {result.error}");
            var d = result.shiftBreak!;
            d.Id = db.Id;
            d.CreatedAt = db.CreatedAt; d.UpdatedAt = db.UpdatedAt;
            return d;
        }
        public static ShiftBreakEntity ToShiftBreakDb(ShiftBreak d, ShiftBreakEntity? db = null)
        {
            db ??= new();
            db.Id = d.Id; db.OrganizationId = d.OrganizationId; db.ShiftId = d.ShiftId;
            db.StartTime = d.StartTime; db.EndTime = d.EndTime;
            db.Type = d.Type.ToString(); db.IsPaid = d.IsPaid;
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }

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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
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
            db.CreatedAt = d.CreatedAt; db.UpdatedAt = d.UpdatedAt ?? d.CreatedAt;
            return db;
        }
        public static Notification ToNotificationDomain(NotificationEntity db) =>
            Notification.Restore(
                id: db.Id,
                organizationId: db.OrganizationId,
                userId: db.UserId,
                ticketId: db.TicketId,
                type: db.Type,
                title: db.Title,
                body: db.Body,
                isRead: db.IsRead,
                createdAt: db.CreatedAt
            );

        public static NotificationEntity ToNotificationDb(Notification domain, NotificationEntity? existing = null)
        {
            var entity = existing ?? new NotificationEntity();

            entity.Id = domain.Id;
            entity.OrganizationId = domain.OrganizationId;
            entity.UserId = domain.UserId;
            entity.TicketId = domain.TicketId;
            entity.Type = domain.Type;
            entity.Title = domain.Title;
            entity.Body = domain.Body;
            entity.IsRead = domain.IsRead;
            entity.CreatedAt = domain.CreatedAt;

            return entity;
        }

        public static ChatMessage ToChatMessageDomain(ChatMessageEntity db)
        {
            var message = ChatMessage.Restore(
                id: db.Id,
                organizationId: db.OrganizationId,
                senderId: db.SenderId,
                senderName: db.Sender?.FullName ?? string.Empty,
                senderAvatar: db.Sender?.AvatarObjectName != null
                    ? $"http://localhost:9000/avatars/{db.Sender.AvatarObjectName}"
                    : null,
                recipientId: db.RecipientId,
                recipientName: db.Recipient?.FullName,
                chatRoomId: db.ChatRoomId,
                text: db.Text,
                sentAt: db.SentAt,
                isRead: db.IsRead,
                createdAt: db.CreatedAt
            );

            message.MessageType = db.MessageType;
            message.AttachmentObject = db.AttachmentObjectName;
            message.AttachmentName = db.AttachmentName;
            message.AttachmentSize = db.AttachmentSize;
            message.AttachmentMime = db.AttachmentMime;
            message.AttachmentUrl = db.AttachmentObjectName != null
                ? $"http://localhost:9000/chat-attachments/{db.AttachmentObjectName}"
                : null;

            return message;
        }

        public static ChatMessageEntity ToChatMessageDb(ChatMessage domain, ChatMessageEntity? existing = null)
        {
            var entity = existing ?? new ChatMessageEntity();

            entity.Id = domain.Id;
            entity.OrganizationId = domain.OrganizationId;
            entity.SenderId = domain.SenderId;
            entity.RecipientId = domain.RecipientId;
            entity.ChatRoomId = domain.ChatRoomId;
            entity.Text = domain.Text;
            entity.SentAt = domain.SentAt;
            entity.IsRead = domain.IsRead;
            entity.CreatedAt = domain.CreatedAt;
            entity.MessageType = domain.MessageType;
            entity.AttachmentObjectName = domain.AttachmentObject;
            entity.AttachmentName = domain.AttachmentName;
            entity.AttachmentSize = domain.AttachmentSize;
            entity.AttachmentMime = domain.AttachmentMime;

            return entity;
        }

        public static TicketComment ToTicketCommentDomain(TicketCommentEntity db, string minioBase) =>
            TicketComment.Restore(
                id: db.Id,
                organizationId: db.OrganizationId,
                ticketId: db.TicketId,
                authorId: db.AuthorId,
                authorName: db.Author?.FullName ?? string.Empty,
                text: db.Text,
                createdAt: db.CreatedAt,
                updatedAt: db.UpdatedAt,
                attachments: db.Attachments?
                    .Select(a => ToTicketAttachmentDomain(a, minioBase))
                    .ToList() ?? []
            );

        public static TicketAttachment ToTicketAttachmentDomain(TicketAttachmentEntity db, string minioBase) =>
            TicketAttachment.Restore(
                id: db.Id,
                organizationId: db.OrganizationId,
                ticketId: db.TicketId,
                commentId: db.CommentId,
                uploaderId: db.UploaderId,
                fileName: db.FileName,
                objectName: db.ObjectName,
                contentType: db.ContentType,
                sizeBytes: db.SizeBytes,
                createdAt: db.CreatedAt,
                url: $"{minioBase}/ticket-attachments/{db.ObjectName}"
            );

        public static TicketAttachmentEntity ToTicketAttachmentDb(TicketAttachment domain)
        {
            return new TicketAttachmentEntity
            {
                Id = domain.Id,
                OrganizationId = domain.OrganizationId,
                TicketId = domain.TicketId,
                CommentId = domain.CommentId,
                UploaderId = domain.UploaderId,
                FileName = domain.FileName,
                ObjectName = domain.ObjectName,
                ContentType = domain.ContentType,
                SizeBytes = domain.SizeBytes,
                CreatedAt = domain.CreatedAt
            };
        }

        public static TicketCommentEntity ToTicketCommentDb(TicketComment domain)
        {
            return new TicketCommentEntity
            {
                Id = domain.Id,
                OrganizationId = domain.OrganizationId,
                TicketId = domain.TicketId,
                AuthorId = domain.AuthorId,
                Text = domain.Text,
                CreatedAt = domain.CreatedAt,
                UpdatedAt = domain.UpdatedAt ?? domain.CreatedAt
            };
        }
    }
}