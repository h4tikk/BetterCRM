namespace BetterCRM.DataAccess.Entities
{
    public class OrganizationEntity : BaseDbEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public Guid? MainDirectorId { get; set; }
        public UserEntity? MainDirector { get; set; }
        public ICollection<DepartmentEntity> Departments { get; set; } = new List<DepartmentEntity>();
        public ICollection<PositionEntity> Positions { get; set; } = new List<PositionEntity>();
        public ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
        public ICollection<ShiftEntity> Shifts { get; set; } = new List<ShiftEntity>();
        public ICollection<WorkSessionEntity> WorkSessions { get; set; } = new List<WorkSessionEntity>();
        public ICollection<TimeLogEntity> TimeLogs { get; set; } = new List<TimeLogEntity>();
        public ICollection<TicketEntity> Tickets { get; set; } = new List<TicketEntity>();
        public ICollection<TicketParticipantEntity> Participants { get; set; } = new List<TicketParticipantEntity>();
        public ICollection<PayrollRecordEntity> PayrollRecords { get; set; } = new List<PayrollRecordEntity>();
    }
}
