using BetterCRM.Core.Interfaces.Services;
using BetterCRM.DataAccess;

namespace BetterCRM.Api.Middleware
{
    public class OrganizationMiddleware
    {
        private readonly RequestDelegate _next;

        public OrganizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ApplicationDbContext dbContext,
            ICurrentUserProvider userProvider)
        {
            var path = context.Request.Path;

            var isPublicPath =
                path.StartsWithSegments("/api/auth") ||
                path.StartsWithSegments("/openapi") ||
                path.StartsWithSegments("/scalar");

            if (isPublicPath)
            {
                await _next(context);
                return;
            }

            var currentUser = userProvider.GetCurrent();

            if (currentUser is not null)
            {
                dbContext.CurrentOrganizationId = currentUser.OrganizationId;
                await _next(context);
                return;
            }


            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Требуется авторизация"
            });
        }
    }
}