using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/chat")]
    public class ChatController : BaseApiController
    {
        private readonly IChatRepository _chatRepo;
        public ChatController(IChatRepository chatRepo, ICurrentUserProvider currentUser)
            : base(currentUser) => _chatRepo = chatRepo;

        [HttpGet("private/{userId:guid}")]
        public async Task<IActionResult> GetPrivateHistory(
        Guid userId,
        [FromQuery] int take = 50,
        [FromQuery] DateTime? before = null)
        {
            var messages = await _chatRepo.GetPrivateMessagesAsync(
                UserId, userId, take, before ?? DateTime.UtcNow);

            return Ok(messages);
        }

        [HttpGet("department/{departmentId:guid}")]
        public async Task<IActionResult> GetDepartmentHistory(
        Guid departmentId,
        [FromQuery] int take = 50,
        [FromQuery] DateTime? before = null)
        {
            var belongs = await _chatRepo.UserBelongsToDepartmentAsync(UserId, departmentId);
            if (!belongs) return Forbid();

            var messages = await _chatRepo.GetDepartmentMessagesAsync(
                departmentId, take, before ?? DateTime.UtcNow);

            return Ok(messages);
        }

        [HttpPost("private/{messageId:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid messageId)
        {
            await _chatRepo.MarkAsReadAsync(messageId, UserId);
            return NoContent();
        }
    }
}
