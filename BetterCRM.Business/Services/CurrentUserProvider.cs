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

            var rawId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(rawId, out var userId))
                throw new InvalidOperationException(
                    $"Claim NameIdentifier отсутствует или имеет невалидный формат: '{rawId}'");

            var rawOrgId = user.FindFirst("OrganizationId")?.Value;
            if (!Guid.TryParse(rawOrgId, out var orgId) || orgId == Guid.Empty)
                throw new InvalidOperationException(
                    $"Claim OrganizationId отсутствует или невалиден: '{rawOrgId}'");

            Guid? departmentId = Guid.TryParse(
                user.FindFirst("DepartmentId")?.Value, out var deptId) && deptId != Guid.Empty
                ? deptId
                : null;

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrWhiteSpace(role))
                throw new InvalidOperationException("Claim Role отсутствует в токене");

            return new CurrentUserInfo(
                Id: userId,
                Email: user.FindFirst(ClaimTypes.Email)?.Value ?? "",
                FullName: user.FindFirst(ClaimTypes.Name)?.Value ?? "",
                Role: role,
                OrganizationId: orgId,
                DepartmentId: departmentId,
                IsMainDirector: user.FindFirst("IsMainDirector")?.Value == "true"
            );
        }
    }
}