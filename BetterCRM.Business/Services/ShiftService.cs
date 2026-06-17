using BetterCRM.Business.Exceptions;
using BetterCRM.Core.Constants;
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
        private readonly IShiftBreakRepository _breakRepo;
        private readonly IUserRepository _userRepo;

        public ShiftService(IShiftRepository shiftRepo, IShiftBreakRepository breakRepo, IUserRepository userRepo)
        {
            _shiftRepo = shiftRepo;
            _breakRepo = breakRepo;
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

        public async Task<ShiftBreak> AddBreakAsync(Guid shiftId, AddBreakCommand cmd, string actorRole, Guid? actorDeptId)
        {
            var shift = await _shiftRepo.GetWithBreaksAsync(shiftId)
                ?? throw new NotFoundException("Смена не найдена");

            await ValidateAccessAsync(actorRole, actorDeptId, shift.UserId);

            var (shiftBreak, err) = shift.AddBreak(cmd.StartTime, cmd.EndTime, cmd.Type, cmd.IsPaid);
            if (shiftBreak == null) throw new DomainException(err!);

            return await _breakRepo.AddAsync(shiftBreak);
        }

        public async Task RemoveBreakAsync(Guid breakId, string actorRole, Guid? actorDeptId)
        {
            var shiftBreak = await _breakRepo.GetByIdAsync(breakId)
                ?? throw new NotFoundException("Перерыв не найден");

            var shift = await _shiftRepo.GetByIdAsync(shiftBreak.ShiftId)
                ?? throw new NotFoundException("Смена не найдена");

            await ValidateAccessAsync(actorRole, actorDeptId, shift.UserId);

            await _breakRepo.DeleteAsync(breakId);
        }

        private async Task ValidateAccessAsync(string actorRole, Guid? actorDeptId, Guid targetUserId)
        {
            var targetUser = await _userRepo.GetByIdAsync(targetUserId)
               ?? throw new NotFoundException("Пользователь не найден");

            if (actorRole == Roles.OrganizationHead)
                return;

            if (actorRole == Roles.DepartmentHead)
            {
                if (!actorDeptId.HasValue || targetUser.DepartmentId != actorDeptId.Value)
                    throw new UnauthorizedOperationException("Нельзя управлять сменами другого отдела");

                return;
            }

            throw new UnauthorizedOperationException("Недостаточно прав");
        }
    }
}