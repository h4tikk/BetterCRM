using BetterCRM.Business.Exceptions;
using BetterCRM.Business.Helpers;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using Microsoft.Extensions.Configuration;
using static BetterCRM.Business.Exceptions.DomainException;


namespace BetterCRM.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _jwtHelper = new JwtHelper(config);
        }

        public async Task<AuthResult?> LoginAsync(LoginCommand cmd)
        {
            var user = await _userRepo.GetByEmailAsync(cmd.Email)
                ?? throw new UnauthorizedOperationException("Неверный email или пароль");

            if (!user.VerifyPassword(cmd.Password))
                throw new UnauthorizedOperationException("Неверный email или пароль");

            if (!user.IsActive)
                throw new UnauthorizedOperationException("Учетная запись заблокирована");

            var info = new CurrentUserInfo(user.Id, user.Email, user.FullName, user.Role, user.OrganizationId, user.DepartmentId, false); 
            return new AuthResult(info, _jwtHelper.GenerateToken(info));
        }

        public async Task<CurrentUserInfo> RegisterAsync(RegisterCommand cmd)
        {
            if (await _userRepo.EmailExistsAsync(cmd.Email))
                throw new ConflictException("Email уже занят");

            var (user, err) = User.Create(cmd.OrganizationId, cmd.Email, cmd.Password, cmd.FullName, cmd.Role, cmd.PositionId, cmd.DepartmentId);
            if (err != null) throw new DomainException(err);

            await _userRepo.AddAsync(user);
            return new CurrentUserInfo(user.Id, user.Email, user.FullName, user.Role, user.OrganizationId, user.DepartmentId, false);
        }

        public Task<CurrentUserInfo?> ValidateTokenAsync(string token) => Task.FromResult<CurrentUserInfo?>(null); 
    }
}
