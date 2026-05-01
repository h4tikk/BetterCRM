using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IWorkSessionRepository : IRepository<WorkSession>
    {
        Task<WorkSession?> GetActiveSessionAsync(Guid userId);
        Task<List<WorkSession>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null);
        Task<decimal> GetTotalHoursAsync(Guid userId, DateTime from, DateTime to);
        Task<Dictionary<DateTime, decimal>> GetHoursByDayAsync(Guid userId, DateTime from, DateTime to);
        Task<List<WorkSession>> GetByShiftAsync(Guid shiftId);

    }
}
