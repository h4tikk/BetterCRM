using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IShiftRepository : IRepository<Shift>
    {
        Task<List<Shift>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null);
        Task<List<Shift>> GetByDepartmentAsync(Guid departmentId, DateTime from, DateTime to);
        Task<Shift?> GetByUserAndDateAsync(Guid userId, DateTime date);
        Task<Shift?> GetWithBreaksAsync(Guid shiftId);
        Task<decimal> GetTotalScheduledHoursAsync(Guid userId, DateTime from, DateTime to);
        Task<decimal> GetTotalAttendancePenaltyAsync(Guid userId, DateTime from, DateTime to);
        Task<List<Shift>> GetForOrganizationAsync(Guid orgId, DateTime from, DateTime to);
    }
}
