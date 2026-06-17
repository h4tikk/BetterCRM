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

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orgClaim = user.FindFirst("OrganizationId")?.Value;
            var deptClaim = user.FindFirst("DepartmentId")?.Value;

            if (!Guid.TryParse(idClaim, out var userId))
                return null;

            if (!Guid.TryParse(orgClaim, out var organizationId))
                return null;

            Guid? departmentId = null;
            if (Guid.TryParse(deptClaim, out var parsedDepartmentId))
                departmentId = parsedDepartmentId;

            return new CurrentUserInfo(
                Id: userId,
                Email: user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                FullName: user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                Role: user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                OrganizationId: organizationId,
                DepartmentId: departmentId,
                IsMainDirector: user.FindFirst("IsMainDirector")?.Value == "true"
            );
        }
    }
}