using BetterCRM.Business.Exceptions;
using BetterCRM.Core.Constants;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepo;
        private readonly IDepartmentRepository _deptRepo;
        private readonly IPositionRepository _positionRepo;
        public UserManagementService(IUserRepository userRepo, IDepartmentRepository deptRepo, IPositionRepository positionRepo)
        {
            _userRepo = userRepo;
            _deptRepo = deptRepo;
            _positionRepo = positionRepo;
        }

        public async Task<User> CreateUserAsync(CurrentUserInfo creator, CreateUserCommand command)
        {
            if(creator.Role == Roles.DepartmentHead)
            {
                if (command.Role != Roles.Employee)
                    throw new ForbiddenException("Руководитель отдела может создавать только сотрудников");
                if (command.DepartmentId != creator.DepartmentId)
                    throw new ForbiddenException("Нельзя добавлять сотрудников в чужой отдел");
            }
            else if (creator.Role == Roles.OrganizationHead)
            {
                if (command.Role == Roles.OrganizationHead)
                    throw new ForbiddenException("Нельзя создать ещё одного руководителя организации");
            }
            else
            {
                throw new ForbiddenException("Недостаточно прав для создания пользователей");
            }

            if (await _userRepo.EmailExistsAsync(command.Email))
                throw new ConflictException("Пользователь с таким email уже существует");

            if (command.Role == Roles.Employee && command.DepartmentId is null)
                throw new DomainException("Для сотрудника необходимо указать отдел");

            if (command.DepartmentId is not null)
            {
                var dept = await _deptRepo.GetByIdAsync(command.DepartmentId.Value);
                if (dept is null || dept.OrganizationId != creator.OrganizationId)
                    throw new NotFoundException("Отдел не найден в данной организации");
            }
            var position = await _positionRepo.GetByIdAsync(command.PositionId);
            if (position is null || position.OrganizationId != creator.OrganizationId)
                throw new NotFoundException("Должность не найдена в данной организации");

            var (user, err) = User.Create(creator.OrganizationId, command.Email, command.Password, command.FullName, command.Role, command.PositionId, command.DepartmentId);

            if (user is null) throw new DomainException(err!);

            await _userRepo.AddAsync(user);
            return user;
        }
    }
}
