using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IShiftRepository : IRepository<Shift>
    {
        Task<List<Shift>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null);
        Task<List<Shift>> GetByDepartmentAsync(Guid departmentId, DateTime from, DateTime to);
        Task<Shift?> GetByUserAndDateAsync(Guid userId, DateTime date);
        Task<decimal> GetTotalScheduledHoursAsync(Guid userId, DateTime from, DateTime to);
        // Суммарные штрафные часы из смен пользователя за период
        Task<decimal> GetTotalAttendancePenaltyAsync(Guid userId, DateTime from, DateTime to);
        Task<List<Shift>> GetForOrganizationAsync(DateTime from, DateTime to);
    }
}
