using BetterCRM.Business.Exceptions;
using BetterCRM.Core.Extensions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepo;
        private readonly IUserRepository _userRepo;

        public ShiftService(IShiftRepository shiftRepo, IUserRepository userRepo)
        {
            _shiftRepo = shiftRepo;
            _userRepo = userRepo;
        }

        public async Task<Shift> CreateAsync(CreateShiftCommand cmd, Guid creatorId, string creatorRole, Guid? creatorDeptId)
        {
            await ValidateAccessAsync(creatorRole, creatorDeptId, cmd.UserId);

            var user = await _userRepo.GetByIdAsync(cmd.UserId)
                ?? throw new NotFoundException("Пользователь не найден");

            if (await _shiftRepo.GetByUserAndDateAsync(cmd.UserId, cmd.Date) != null)
                throw new ConflictException("Смена на эту дату уже существует");

            var (shift, err) = Shift.Create(user.OrganizationId, cmd.UserId, cmd.Date, cmd.StartTime, cmd.EndTime);
            if (err != null) throw new DomainException(err);

            return await _shiftRepo.AddAsync(shift!);
        }

        public async Task<List<Shift>> GetForDepartmentAsync(Guid departmentId, DateTime from, DateTime to) =>
            await _shiftRepo.GetByDepartmentAsync(departmentId, from, to);

        public async Task<List<Shift>> GetForOrganizationAsync(Guid orgId, DateTime from, DateTime to) =>
            await _shiftRepo.GetForOrganizationAsync(orgId, from, to);

        public async Task<List<Shift>> GetForUserAsync(Guid userId, DateTime from, DateTime to) =>
            await _shiftRepo.GetByUserAsync(userId, from, to);

        public async Task<Shift?> GetTodayShiftAsync(Guid userId) =>
            await _shiftRepo.GetByUserAndDateAsync(userId, DateTime.UtcNow.Date);

        public async Task UpdateAsync(Guid shiftId, UpdateShiftCommand cmd, Guid updaterId, string updaterRole, Guid? updaterDeptId)
        {
            var shift = await _shiftRepo.GetByIdAsync(shiftId)
                ?? throw new NotFoundException("Смена не найдена");

            await ValidateAccessAsync(updaterRole, updaterDeptId, shift.UserId);

            if (cmd.StartTime.HasValue && cmd.EndTime.HasValue)
                shift.UpdateTime(cmd.StartTime.Value, cmd.EndTime.Value);

            if (cmd.Status == ShiftStatus.Cancelled) shift.Cancel();
            if (cmd.Status == ShiftStatus.Completed) shift.Complete();

            await _shiftRepo.UpdateAsync(shift);
        }

        private async Task ValidateAccessAsync(string actorRole, Guid? actorDeptId, Guid targetUserId)
        {
            if (actorRole != "OrganizationHead" && actorRole != "DepartmentHead")
                throw new UnauthorizedOperationException("Недостаточно прав");

            if (actorRole == "DepartmentHead" && actorDeptId.HasValue)
            {
                var target = await _userRepo.GetByIdAsync(targetUserId)
                    ?? throw new NotFoundException("Пользователь не найден");

                if (target.DepartmentId != actorDeptId.Value)
                    throw new UnauthorizedOperationException("Нельзя управлять сменами другого отдела");
            }
        }
    }
}