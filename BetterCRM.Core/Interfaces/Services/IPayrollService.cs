using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<PayrollRecord> CalculateForUserAsync(Guid userId, int year, int month);
        Task<List<PayrollRecord>> CalculateForDepartmentAsync(Guid departmentId, int year, int month);
        Task<PayrollRecord?> GetRecordAsync(Guid userId, int year, int month);
        Task<PayrollPreviewDto> PreviewForUserAsync(Guid userId, int year, int month);
    }
    public record PayrollPreviewDto
    {
        public Guid UserId { get; init; }
        public string Period { get; init; } = string.Empty;
        public decimal ScheduledHours { get; init; }
        public decimal ActualHours { get; init; }
        public decimal AttendancePenaltyHours { get; init; }
        public decimal TicketPenaltyHours { get; init; }
        public decimal TotalPenaltyHours { get; init; }
        public decimal FinalBillableHours { get; init; }
        public decimal HourlyRate { get; init; }
        public decimal EstimatedSalary { get; init; }
    }
}
