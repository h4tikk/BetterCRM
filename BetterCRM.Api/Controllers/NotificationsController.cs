using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    [Authorize]
    public class NotificationsController : BaseApiController
    {
        private readonly INotificationRepository _repo;

        public NotificationsController(INotificationRepository repo, ICurrentUserProvider currentUser)
            : base(currentUser) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _repo.GetPagedAsync(UserId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> UnreadCount()
        {
            var count = await _repo.GetUnreadCountAsync(UserId);
            return Ok(new { count });
        }

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            await _repo.MarkAsReadAsync(id, UserId);
            return NoContent();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            await _repo.MarkAllAsReadAsync(UserId);
            return NoContent();
        }
    }
}
