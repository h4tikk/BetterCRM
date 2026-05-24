using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly ICurrentUserProvider CurrentUser;
        private CurrentUserInfo? _currentUserCache;

        protected BaseApiController(ICurrentUserProvider currentUser)
            => CurrentUser = currentUser;

        private CurrentUserInfo Current =>
            _currentUserCache ??= CurrentUser.GetCurrent()
                ?? throw new UnauthorizedAccessException();

        protected Guid UserId => Current.Id;
        protected string UserRole => Current.Role;
        protected Guid? UserDeptId => Current.DepartmentId;
        protected Guid OrgId => Current.OrganizationId;
    }
}
