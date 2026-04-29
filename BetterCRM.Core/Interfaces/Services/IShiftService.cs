using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record CreateShiftCommand(Guid UserId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime);
    public record UpdateShiftCommand(TimeSpan? StartTime, TimeSpan? EndTime, string? Status);
    public interface IShiftService
    {
        Task<Shift> CreateAsync(CreateShiftCommand cmd, Guid creatorId, string creatorRole, Guid? creatorDeptId);
        Task<List<Shift>> GetForUserAsync(Guid userId, DateTime from, DateTime to);
        Task<List<Shift>> GetForDepartmentAsync(Guid departmentId, DateTime date);
        Task UpdateAsync(Guid shiftId, UpdateShiftCommand cmd, Guid updaterId, string updaterRole, Guid? updaterDeptId);
    }
}
