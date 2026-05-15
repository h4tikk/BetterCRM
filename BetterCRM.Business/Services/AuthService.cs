using BetterCRM.Business.Exceptions;
using BetterCRM.Business.Helpers;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IPositionRepository _positionRepo;
        private readonly JwtHelper _jwt;

        public AuthService(IUserRepository userRepo, IOrganizationRepository orgRepo, IPositionRepository positionRepo, JwtHelper jwt)
        {
            _userRepo = userRepo;
            _orgRepo = orgRepo;
            _positionRepo = positionRepo;
            _jwt = jwt;
        }

        public async Task<AuthResult?> LoginAsync(LoginCommand command)
        {
            var user = await _userRepo.GetByEmailAsync(command.Email)
                ?? throw new UnauthorizedOperationException("Неверный email или пароль");

            if(!user.VerifyPassword(command.Password))
            {
                throw new UnauthorizedOperationException("Неверный email или пароль");
            }
            if(!user.IsActive)
                throw new UnauthorizedOperationException("Учётная запись заблокирована");

            var org = await _orgRepo.GetByIdAsync(user.OrganizationId);
            var info = BuildUserInfo(user, org);

            return new AuthResult(info, _jwt.GenerateToken(info));

        }

        private CurrentUserInfo BuildUserInfo(User user, Organization? org) =>
            new(user.Id, user.Email, user.FullName, user.Role, user.OrganizationId,user.DepartmentId, org?.IsMainDirector(user.Id) ?? false );

        public async Task<AuthResult> RegisterAsync(RegisterCommand command)
        {
            if(await _userRepo.EmailExistsAsync(command.Email))
                throw new ConflictException("Пользователь с таким email уже существует");

            var (org, orgErr) = Organization.Create(command.OrganizationName);
            if (org is null) throw new InvalidOperationException(orgErr!);

            var savedOrg = await _orgRepo.AddAsync(org)
                ?? throw new InvalidOperationException("Не удалось сохранить организацию");

            var (position, posErr) = Position.Create(savedOrg.Id, command.PositionTitle, 0, 8);
            if (position is null) throw new InvalidOperationException(posErr!);
            var savedPos = await _positionRepo.AddAsync(position);

            var (user, userErr) = User.Create(savedOrg.Id, command.Email, command.Password, command.FullName, "OrgHead", savedPos.Id, null);
            if(user is null) throw new InvalidOperationException(userErr!);

            savedOrg.AssignMainDirector(user.Id);
            await _orgRepo.UpdateAsync(savedOrg);

            var info = BuildUserInfo(user, savedOrg);
            return new AuthResult(info, _jwt.GenerateToken(info));
        }
    }
}