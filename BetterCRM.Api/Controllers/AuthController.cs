using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(
        string Email, string Password, string FullName, string Role,
        Guid OrganizationId, Guid PositionId, Guid? DepartmentId);

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        { 
            var result = await _auth.LoginAsync(new LoginCommand(req.Email, req.Password));
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var user = await _auth.RegisterAsync(new RegisterCommand(
                req.Email, req.Password, req.FullName, req.Role,
                req.OrganizationId, req.PositionId, req.DepartmentId));
            return CreatedAtAction(null, user);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me([FromServices] ICurrentUserProvider up)
        {
            var user = up.GetCurrent();
            if (user == null) return Unauthorized();
            return Ok(user);
        }
    }
}
