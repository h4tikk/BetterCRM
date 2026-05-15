using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    public record CreateUserRequest(
       string FullName,
       string Email,
       string Password,
       string Role,
       Guid PositionId,
       Guid? DepartmentId
   );
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userService;
        private readonly ICurrentUserProvider _currentUser;

        public UsersController(IUserManagementService userService, ICurrentUserProvider currentUser)
        {
            _userService = userService;
            _currentUser = currentUser;
        }

        [HttpPost]
        [Authorize(Roles = "OrganizationHead,DepartmentHead")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
        {
            var creator = _currentUser.GetCurrent();
            var user = await _userService.CreateUserAsync(creator, new CreateUserCommand(
                req.FullName, req.Email, req.Password, req.Role, req.PositionId, req.DepartmentId));

            return Ok(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role,
                departmentId = user.DepartmentId,
                positionId = user.PositionId,
                hireDate = DateTime.UtcNow
            });
        }
    }
}
