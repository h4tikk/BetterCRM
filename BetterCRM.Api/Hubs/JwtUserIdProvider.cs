using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BetterCRM.Api.Hubs
{
    public class JwtUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection) =>
            connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
