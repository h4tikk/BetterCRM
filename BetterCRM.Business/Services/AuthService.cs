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
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepo, JwtHelper jwtHelper)
        {
            _userRepo = userRepo;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResult> LoginAsync(LoginCommand cmd)
        {

            var user = await _userRepo.GetByEmailAsync(cmd.Email);

            var passwordValid = user != null && user.VerifyPassword(cmd.Password);

            if (user == null || !passwordValid)
            {
                await Task.Delay(300);
                throw new UnauthorizedOperationException("Неверный email или пароль");
            }

            if (!user.IsActive)
                throw new UnauthorizedOperationException("Учётная запись заблокирована");

            var info = new CurrentUserInfo(
                user.Id,
                user.Email,
                user.FullName,
                user.Role,
                user.OrganizationId,
                user.DepartmentId,
                false);

            return new AuthResult(info, _jwtHelper.GenerateToken(info));
        }

        public async Task<CurrentUserInfo> RegisterAsync(RegisterCommand cmd)
        {
            if (await _userRepo.EmailExistsAsync(cmd.Email))
                throw new ConflictException("Email уже занят");

            var (user, err) = User.Create(
                cmd.OrganizationId,
                cmd.Email,
                cmd.Password,
                cmd.FullName,
                cmd.Role,
                cmd.PositionId,
                cmd.DepartmentId);

            if (err != null) throw new DomainException(err);

            await _userRepo.AddAsync(user!);

            return new CurrentUserInfo(
                user!.Id,
                user.Email,
                user.FullName,
                user.Role,
                user.OrganizationId,
                user.DepartmentId,
                false);
        }

    }
}