using BetterCRM.Core.Interfaces.Services;
using BetterCRM.DataAccess;

namespace BetterCRM.Api.Middleware
{
    public class OrganizationMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly string[] _publicPaths =
        [
            "/api/auth",
            "/health",
            "/swagger"
        ];

        public OrganizationMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            ApplicationDbContext dbContext,
            ICurrentUserProvider userProvider)
        {
            var isPublicPath = _publicPaths.Any(p =>
                context.Request.Path.StartsWithSegments(p));

            if (isPublicPath)
            {
                await _next(context);
                return;
            }

            var user = userProvider.GetCurrent();

            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Требуется авторизация" });
                return;
            }

            if (user.OrganizationId == Guid.Empty)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "Пользователь не привязан к организации" });
                return;
            }

            dbContext.CurrentOrganizationId = user.OrganizationId;

            await _next(context);
        }
    }

    public static class OrganizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseOrganizationContext(this IApplicationBuilder app)
            => app.UseMiddleware<OrganizationMiddleware>();
    }
}