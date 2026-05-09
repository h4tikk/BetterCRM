using BetterCRM.Core.Extensions;
using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    public record CreateTicketRequest(
        string Title, string? Description, TicketPriority Priority,
        Guid? DepartmentId, Guid? AssigneeId);

    public record AddParticipantRequest(Guid UserId, string Role);

    [Authorize]
    public class TicketsController : BaseApiController
    {
        private readonly ITicketService _tickets;

        public TicketsController(ITicketService tickets, ICurrentUserProvider up)
            : base(up) => _tickets = tickets;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest req)
        {
            var ticket = await _tickets.CreateAsync(new CreateTicketCommand(
                req.Title, req.Description, req.Priority,
                UserId, req.DepartmentId, req.AssigneeId));
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _tickets.GetByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _tickets.GetFilteredAsync(UserId, UserRole, UserDeptId);
            return Ok(tickets);
        }

        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _tickets.ApproveAsync(id, UserId);
            return NoContent();
        }

        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _tickets.RejectAsync(id, UserId);
            return NoContent();
        }

        [HttpPost("{id:guid}/resolve")]
        public async Task<IActionResult> Resolve(Guid id)
        {
            await _tickets.ResolveAsync(id, UserId);
            return NoContent();
        }

        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            await _tickets.CloseAsync(id, UserId);
            return NoContent();
        }

        [HttpGet("{id:guid}/participants")]
        public async Task<IActionResult> GetParticipants(Guid id)
        {
            var list = await _tickets.GetParticipantsAsync(id);
            return Ok(list);
        }

        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/participants")]
        public async Task<IActionResult> AddParticipant(
            Guid id, [FromBody] AddParticipantRequest req)
        {
            await _tickets.AddParticipantAsync(id, req.UserId, req.Role, UserId);
            return NoContent();
        }

        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpDelete("{id:guid}/participants/{userId:guid}")]
        public async Task<IActionResult> RemoveParticipant(Guid id, Guid userId)
        {
            await _tickets.RemoveParticipantAsync(id, userId, UserId);
            return NoContent();
        }
    }
}
