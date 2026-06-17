using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Messages;
using MassTransit;
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
        private readonly IFileStorageService _storage;
        private readonly IPublishEndpoint _bus;

        private const string Bucket = "chat-attachments";
        private const long MaxFileSize = 10 * 1024 * 1024;
        private static readonly string[] AllowedImageTypes =
            { "image/jpeg", "image/png", "image/gif", "image/webp" };

        public ChatController(
            IChatRepository chatRepo,
            IFileStorageService storage,
            IPublishEndpoint bus,
            ICurrentUserProvider currentUser)
            : base(currentUser)
        {
            _chatRepo = chatRepo;
            _storage = storage;
            _bus = bus;
        }

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

        [HttpPost("private/{userId:guid}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendPrivateImage(
        Guid userId,
        [FromForm] IFormFile file,
        [FromForm] string? caption = null)
        {
            return await SendImageAsync(file, caption, recipientId: userId, chatRoomId: null);
        }

        [HttpPost("department/{departmentId:guid}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendDepartmentImage(
        Guid departmentId,
        [FromForm] IFormFile file,
        [FromForm] string? caption = null)
        {
            var belongs = await _chatRepo.UserBelongsToDepartmentAsync(UserId, departmentId);
            if (!belongs) return Forbid();

            return await SendImageAsync(file, caption, recipientId: null, chatRoomId: departmentId);
        }

        private async Task<IActionResult> SendImageAsync(
            IFormFile file, string? caption, Guid? recipientId, Guid? chatRoomId)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { error = new { code = "INVALID_FILE", message = "Файл не передан" } });

            if (file.Length > MaxFileSize)
                return BadRequest(new { error = new { code = "FILE_TOO_LARGE", message = "Файл превышает 10 МБ" } });

            if (!AllowedImageTypes.Contains(file.ContentType))
                return BadRequest(new { error = new { code = "INVALID_FILE_TYPE", message = "Допустимы только изображения (jpeg, png, gif, webp)" } });

            var safeName = Path.GetFileName(file.FileName);
            var objectName = $"{UserId}/{Guid.NewGuid()}-{safeName}";
            var url = $"http://localhost:9000/{Bucket}/{objectName}";

            await using var stream = file.OpenReadStream();
            await _storage.UploadAsync(Bucket, objectName, stream, file.ContentType);

            var messageId = Guid.NewGuid();

            await _bus.Publish(new ChatMessageEvent(
                OrganizationId: OrgId,
                MessageId: messageId,
                SenderId: UserId,
                RecipientId: recipientId,
                ChatRoomId: chatRoomId,
                Text: string.IsNullOrWhiteSpace(caption) ? safeName : caption.Trim(),
                SentAt: DateTime.UtcNow,
                MessageType: "image",
                AttachmentObject: objectName,
                AttachmentUrl: url,
                AttachmentName: safeName,
                AttachmentSize: file.Length,
                AttachmentMime: file.ContentType
            ));

            return Accepted(new
            {
                messageId,
                messageType = "image",
                attachmentUrl = url,
                attachmentName = safeName,
                attachmentSize = file.Length,
                attachmentMime = file.ContentType
            });
        }
    }
}
