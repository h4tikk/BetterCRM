using BetterCRM.Core.Extensions;
using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record CreateShiftCommand(Guid UserId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime);
    public record UpdateShiftCommand(TimeSpan? StartTime, TimeSpan? EndTime, ShiftStatus? Status);
    public record AddBreakCommand(TimeSpan StartTime, TimeSpan EndTime, BreakType Type, bool IsPaid);

    public interface IShiftService
    {
        Task<Shift> CreateAsync(CreateShiftCommand cmd, Guid creatorId, string creatorRole, Guid? creatorDeptId);
        Task<Shift?> GetTodayShiftAsync(Guid userId);
        Task<List<Shift>> GetForUserAsync(Guid userId, DateTime from, DateTime to);
        Task<List<Shift>> GetForDepartmentAsync(Guid departmentId, DateTime from, DateTime to);
        Task UpdateAsync(Guid shiftId, UpdateShiftCommand cmd, Guid updaterId, string updaterRole, Guid? updaterDeptId);
        Task<List<Shift>> GetForOrganizationAsync(Guid orgId, DateTime from, DateTime to);
        Task<ShiftBreak> AddBreakAsync(Guid shiftId, AddBreakCommand cmd, string actorRole, Guid? actorDeptId);
        Task RemoveBreakAsync(Guid breakId, string actorRole, Guid? actorDeptId);
    }
}
