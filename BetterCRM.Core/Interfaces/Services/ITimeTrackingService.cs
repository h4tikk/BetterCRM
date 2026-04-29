using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record StartSessionCommand(Guid UserId, Guid? TicketId);
    public record StopSessionCommand(Guid UserId, string? Description);
    public record ShiftAttendanceDto(decimal Scheduled, decimal Actual, decimal Penalty, decimal Overtime);
    public interface ITimeTrackingService
    {
        Task<WorkSession> StartSessionAsync(StartSessionCommand command);
        Task<decimal> StopSessionAsync(StopSessionCommand command);
        Task<WorkSession?> GetActiveSessionAsync(Guid userId);
        Task<List<WorkSession>> GetUserSessionsAsync(Guid userId, DateTime? from = null, DateTime? to = null);
        Task<decimal> GetTodayHoursAsync(Guid userId);
        Task<decimal> GetWeekHoursAsync(Guid userId);
        Task<decimal> GetMonthHoursAsync(Guid userId, int year, int month);
        Task<Dictionary<DateTime, decimal>> GetHoursByDayAsync(Guid userId, DateTime from, DateTime to);
        Task<decimal> GetTotalHoursByTicketAsync(Guid ticketId);
    }
}
