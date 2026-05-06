using BetterCRM.Core.Extensions;
using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public record CreateShiftCommand(Guid UserId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime);
    public record UpdateShiftCommand(TimeSpan? StartTime, TimeSpan? EndTime, ShiftStatus? Status);

    public interface IShiftService
    {
        // Только DepartmentHead может создавать смены своего отдела
        Task<Shift> CreateAsync(CreateShiftCommand cmd, Guid creatorId, string creatorRole, Guid? creatorDeptId);
        // Получить смену пользователя на сегодня (для кнопки "Начать смену")
        Task<Shift?> GetTodayShiftAsync(Guid userId);
        Task<List<Shift>> GetForUserAsync(Guid userId, DateTime from, DateTime to);
        Task<List<Shift>> GetForDepartmentAsync(Guid departmentId, DateTime from, DateTime to);
        Task UpdateAsync(Guid shiftId, UpdateShiftCommand cmd, Guid updaterId, string updaterRole, Guid? updaterDeptId);
        Task<List<Shift>> GetForOrganizationAsync(DateTime from, DateTime to);
    }
}
