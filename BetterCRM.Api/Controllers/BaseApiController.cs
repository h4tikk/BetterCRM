using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ICurrentUserProvider CurrentUser;

        protected BaseApiController(ICurrentUserProvider currentUser)
            => CurrentUser = currentUser;

        protected Guid UserId => CurrentUser.GetCurrent()!.Id;
        protected string UserRole => CurrentUser.GetCurrent()!.Role;
        protected Guid? UserDeptId => CurrentUser.GetCurrent()!.DepartmentId;
        protected Guid OrgId => CurrentUser.GetCurrent()!.OrganizationId;
    }
}
