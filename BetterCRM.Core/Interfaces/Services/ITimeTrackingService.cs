using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record StartSessionCommand(Guid UserId);
    public record StopSessionCommand(Guid UserId, string? Description);

    // DTO для текущего заработка сотрудника
    public record WeekEarningsDto(
        decimal WorkedHours,      // Отработано часов за неделю
        decimal BillableHours,    // Оплачиваемые часы (без переработки)
        decimal HourlyRate,       // Ставка
        decimal CurrentEarnings,  // Текущий заработок (до вычета штрафов)
        decimal PenaltyHours,     // Штрафные часы (опоздания + ранний уход)
        decimal EstimatedNet      // Примерный чистый заработок
    );

    public interface ITimeTrackingService
    {
        // Начать сессию — проверяет наличие смены на сегодня
        Task<WorkSession> StartSessionAsync(StartSessionCommand command);
        Task<decimal> StopSessionAsync(StopSessionCommand command);
        Task<WorkSession?> GetActiveSessionAsync(Guid userId);
        Task<List<WorkSession>> GetUserSessionsAsync(Guid userId, DateTime? from = null, DateTime? to = null);
        Task<decimal> GetTodayHoursAsync(Guid userId);
        Task<decimal> GetWeekHoursAsync(Guid userId);
        Task<decimal> GetMonthHoursAsync(Guid userId, int year, int month);
        Task<Dictionary<DateTime, decimal>> GetHoursByDayAsync(Guid userId, DateTime from, DateTime to);
        Task<decimal> GetTotalHoursByTicketAsync(Guid ticketId);
        // Текущий заработок за неделю для сотрудника
        Task<WeekEarningsDto> GetWeekEarningsAsync(Guid userId);
    }
}
