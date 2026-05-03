using BetterCRM.Core.Interfaces.Services;
using BetterCRM.DataAccess;

namespace BetterCRM.Api.Moddleware
{
    public class OrganizationMiddleware
    {
        private readonly RequestDelegate _next;
        public OrganizationMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, ICurrentUserProvider userProvider)
        {
            var user = userProvider.GetCurrent();

            if(user != null)
                dbContext.CurrentOrganizationId = user.OrganizationId;
            else if(!context.Request.Path.StartsWithSegments("/api/auth"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Требуется авторизация" });
                return;
            }
            await _next(context);
        }
    }
}
