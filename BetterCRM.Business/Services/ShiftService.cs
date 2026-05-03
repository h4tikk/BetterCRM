using BetterCRM.Business.Exceptions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using System.Data;
using static BetterCRM.Business.Exceptions.DomainException;


namespace BetterCRM.Business.Services
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepo;
        private readonly IUserRepository _userRepo;
        private readonly ICurrentUserProvider _currentUser;

        public ShiftService(IShiftRepository shiftRepo, IUserRepository userRepo, ICurrentUserProvider currentUser)
        {
            _shiftRepo = shiftRepo;
            _userRepo = userRepo;
            _currentUser = currentUser;
        }

        public async Task<Shift> CreateAsync(CreateShiftCommand cmd, Guid creatorId, string creatorRole, Guid? creatorDeptId)
        {
            await ValidateAccessAsync(creatorId, creatorRole, creatorDeptId, cmd.UserId);

            var user = await _userRepo.GetByIdAsync(cmd.UserId)
                ?? throw new NotFoundException("Польщователь не найден");

            if (await _shiftRepo.GetByUserAndDateAsync(cmd.UserId, cmd.Date) != null)
                throw new ConflictException("Смена на эту дату уже существует");

            var (shift, err) = Shift.Create(user.OrganizationId, cmd.UserId, cmd.Date, cmd.StartTime, cmd.EndTime);
            if (err != null) throw new DomainException(err);

            return await _shiftRepo.AddAsync(shift);

        }

        public async Task<List<Shift>> GetForDepartmentAsync(Guid departmentId, DateTime date) =>
            await _shiftRepo.GetByDepartmentAsync(departmentId, date);

        public async Task<List<Shift>> GetForUserAsync(Guid userId, DateTime from, DateTime to) =>
            await _shiftRepo.GetByUserAsync(userId, from, to);

        public async Task UpdateAsync(Guid shiftId, UpdateShiftCommand cmd, Guid updaterId, string updaterRole, Guid? updaterDeptId)
        {
            var shift = await _shiftRepo.GetByIdAsync(shiftId) ?? throw new NotFoundException("Смена не найдена");
            await ValidateAccessAsync(updaterId, updaterRole, updaterDeptId, shift.UserId);
            if (cmd.StartTime.HasValue && cmd.EndTime.HasValue) shift.UpdateTime(cmd.StartTime.Value, cmd.EndTime.Value);
            else if (cmd.Status == "Cancelled") shift.Cancel();
            else if (cmd.Status == "Completed") shift.Complete();

            await _shiftRepo.UpdateAsync(shift);
        }
        private async Task ValidateAccessAsync(Guid actorId, string actorRole, Guid? actorDeptId, Guid targetUserId)
        {
            var target = await _userRepo.GetByIdAsync(targetUserId);
            if (target == null) throw new InvalidOperationException("Пользователь не найден");

            if (actorRole == "Admin" || actorRole == "DepartmentHead") { }
            else throw new UnauthorizedAccessException("Недостаточно прав");

            if (actorRole == "DepartmentHead" && actorDeptId.HasValue && target.DepartmentId != actorDeptId.Value)
                throw new UnauthorizedAccessException("Нельзя управлять сменами другого отдела");
        }
    }
}
