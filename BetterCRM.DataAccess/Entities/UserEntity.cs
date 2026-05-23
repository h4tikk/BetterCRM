namespace BetterCRM.DataAccess.Entities
{
    public class UserEntity : TenantDbEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;
        public Guid? DepartmentId {  get; set; }
        public DepartmentEntity? Department { get; set; }
        public Guid PositionId { get; set; }
        public PositionEntity Position { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? AvatarObjectName { get; set; }

        public ICollection<TicketEntity> CreatedTickets { get; set; } = new List<TicketEntity>();
        public ICollection<TicketEntity> AssignedTickets { get; set; } = new List<TicketEntity>();
        public ICollection<WorkSessionEntity> WorkSessions { get; set; } = new List<WorkSessionEntity>();
        public ICollection<TicketParticipantEntity> Participations { get; set; } = new List<TicketParticipantEntity>();
        public ICollection<PayrollRecordEntity> PayrollRecords { get; set; } = new List<PayrollRecordEntity>();
        public ICollection<ShiftEntity> Shifts { get; set; } = new List<ShiftEntity>();
    }

}

