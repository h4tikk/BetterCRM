using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    [Authorize]
    [Route("api/tickets/{ticketId:guid}/comments")]
    public class TicketCommentsController : BaseApiController
    {
        private readonly ITicketCommentService _service;

        public TicketCommentsController(
            ITicketCommentService service,
            ICurrentUserProvider currentUser)
            : base(currentUser) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid ticketId)
        {
            var comments = await _service.GetByTicketAsync(ticketId);
            return Ok(comments);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add(Guid ticketId, [FromForm] string text, [FromForm] IFormFileCollection? files = null)
        {
            var streams = new List<Stream>();
            try
            {
                var attachments = files?
                    .Select(f =>
                    {
                        var s = f.OpenReadStream();
                        streams.Add(s);
                        return (s, f.FileName, f.ContentType, f.Length);
                    })
                    .ToList();

                var (comment, error) = await _service.AddCommentAsync(
                    ticketId: ticketId,
                    authorId: UserId,
                    authorName: CurrentUser.GetCurrent()!.FullName,
                    organizationId: OrgId,
                    text: text,
                    files: attachments);

                if (comment is null) return BadRequest(new { error });
                return Ok(comment);
            }
            finally
            {
                foreach (var s in streams)
                    await s.DisposeAsync();
            }
        }

        [HttpPut("{commentId:guid}")]
        public async Task<IActionResult> Update(
        Guid ticketId,
        Guid commentId,
        [FromBody] UpdateCommentRequest request)
        {
            var (success, error) = await _service.UpdateCommentAsync(commentId, UserId, request.Text);
            if (!success) return BadRequest(new { error });
            return NoContent();
        }

        [HttpDelete("{commentId:guid}")]
        public async Task<IActionResult> Delete(Guid ticketId, Guid commentId)
        {
            var (success, error) = await _service.DeleteCommentAsync(commentId, UserId, UserRole);
            if (!success) return BadRequest(new { error });
            return NoContent();
        }

        [HttpPost("{commentId:guid}/attachments")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddAttachment(
        Guid ticketId,
        Guid commentId,
        [FromForm] IFormFile file)
        {
            var (attachment, error) = await _service.AddAttachmentAsync(
                ticketId: ticketId,
                commentId: commentId,
                uploaderId: UserId,
                organizationId: OrgId,
                stream: file.OpenReadStream(),
                fileName: file.FileName,
                contentType: file.ContentType,
                size: file.Length
            );

            if (attachment is null) return BadRequest(new { error });
            return Ok(attachment);
        }

        [HttpDelete("{commentId:guid}/attachments/{attachmentId:guid}")]
        public async Task<IActionResult> DeleteAttachment(
        Guid ticketId, Guid commentId, Guid attachmentId)
        {
            var (success, error) = await _service.DeleteAttachmentAsync(attachmentId, UserId, UserRole);
            if (!success) return BadRequest(new { error });
            return NoContent();
        }
    }
    public record UpdateCommentRequest(string Text);

}
