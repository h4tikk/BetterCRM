using BetterCRM.Business.Exceptions;
using System.Text.Json;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try { await _next(context); }
            catch (Exception ex) { await HandleExceptionAsync(context, ex); }
        }

        private async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            var (status, code, message) = ex switch
            {
                NotFoundException e => (404, "NOT_FOUND", e.Message),
                ConflictException e => (409, "CONFLICT", e.Message),
                UnauthorizedOperationException e => (403, "FORBIDDEN", e.Message),
                DomainException e => (422, "DOMAIN_ERROR", e.Message),
                UnauthorizedAccessException => (401, "UNAUTHORIZED", "Требуется авторизация"),
                _ => (500, "INTERNAL_ERROR", "Внутренняя ошибка сервера")
            };

            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new
            {
                error = new { code, message }
            });
            await ctx.Response.WriteAsync(body);
        }
    }
}
