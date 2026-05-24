using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(
        string OrganizationName,
        string FullName,
        string Email,
        string Password,
        string PositionTitle);

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ICurrentUserProvider _currentUser;
        public AuthController(IAuthService auth, ICurrentUserProvider currentUser) { _auth = auth; _currentUser = currentUser; }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        { 
            var result = await _auth.LoginAsync(new LoginCommand(req.Email, req.Password));
            return Ok(new
            {
                token = result!.Token,
                userId = result.User.Id,
                fullName = result.User.FullName,
                role = result.User.Role,
                organizationId = result.User.OrganizationId,
                departmentId = result.User.DepartmentId,
                isMainDirector = result.User.IsMainDirector,
            });

        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var res = await _auth.RegisterAsync(new RegisterCommand(
                req.OrganizationName, req.FullName, req.Email, req.Password,req.PositionTitle));
            return Ok(new
            {
                token = res.Token,
                userId = res.User.Id,
                fullName = res.User.FullName,
                role = res.User.Role,
                organizationId = res.User.OrganizationId,
                isMainDirector = res.User.IsMainDirector,
            });
        }

        [Authorize]
        [HttpGet("me")]
        public Task<IActionResult> Me([FromServices] ICurrentUserProvider up)
        {
            var user = _currentUser.GetCurrent();
            if (user == null) return Task.FromResult<IActionResult>(Unauthorized());
            return Task.FromResult<IActionResult>(Ok(user));
        }
    }
}
