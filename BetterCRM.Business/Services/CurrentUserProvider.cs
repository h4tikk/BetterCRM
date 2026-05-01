using Microsoft.AspNetCore.Http;
using BetterCRM.Core.Interfaces.Services;
using System.Security.Claims;

namespace BetterCRM.Business.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _http;
        public CurrentUserProvider(IHttpContextAccessor http) => _http = http;

        public CurrentUserInfo? GetCurrent()
        {
            var user = _http.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true) return null;

            return new CurrentUserInfo(
                Id: Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                Email: user.FindFirst(ClaimTypes.Email)?.Value ?? "",
                FullName: user.FindFirst(ClaimTypes.Name)?.Value ?? "",
                Role: user.FindFirst(ClaimTypes.Role)?.Value ?? "Employee",
                OrganizationId: Guid.TryParse(user.FindFirst("OrganizationId")?.Value, out var orgId) ? orgId : Guid.Empty,
                DepartmentId: Guid.TryParse(user.FindFirst("DepartmentId")?.Value, out var deptId) ? deptId : null,
                IsMainDirector: user.FindFirst("IsMainDirector")?.Value == "true"
            );
        }
    }
}
