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

        /// <summary>
        /// Создать тикет (любой авторизованный пользователь).
        /// Тикет создаётся в статусе Draft — требует подтверждения DepartmentHead.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest req)
        {
            var ticket = await _tickets.CreateAsync(new CreateTicketCommand(
                req.Title, req.Description, req.Priority,
                UserId, req.DepartmentId, req.AssigneeId));
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        /// <summary>Получить тикет по ID</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _tickets.GetByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        /// <summary>
        /// Список тикетов с учётом роли:
        /// - Employee: свои (созданные + назначенные + участник), без чужих Draft
        /// - DepartmentHead: все тикеты своего отдела (включая Draft)
        /// - OrganizationHead/Admin: все тикеты организации
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _tickets.GetFilteredAsync(UserId, UserRole, UserDeptId);
            return Ok(tickets);
        }

        /// <summary>
        /// Подтвердить тикет (Draft → Open).
        /// Только DepartmentHead своего отдела или Admin.
        /// </summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _tickets.ApproveAsync(id, UserId);
            return NoContent();
        }

        /// <summary>
        /// Отклонить тикет (Draft → Closed).
        /// Только DepartmentHead своего отдела или Admin.
        /// </summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            await _tickets.RejectAsync(id, UserId);
            return NoContent();
        }

        /// <summary>Пометить тикет как решённый (исполнитель или участник)</summary>
        [HttpPost("{id:guid}/resolve")]
        public async Task<IActionResult> Resolve(Guid id)
        {
            await _tickets.ResolveAsync(id, UserId);
            return NoContent();
        }

        /// <summary>Закрыть тикет (только DepartmentHead или Admin)</summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/close")]
        public async Task<IActionResult> Close(Guid id)
        {
            await _tickets.CloseAsync(id, UserId);
            return NoContent();
        }

        /// <summary>Список участников тикета</summary>
        [HttpGet("{id:guid}/participants")]
        public async Task<IActionResult> GetParticipants(Guid id)
        {
            var list = await _tickets.GetParticipantsAsync(id);
            return Ok(list);
        }

        /// <summary>
        /// Добавить участника в тикет.
        /// Только DepartmentHead своего отдела или Admin.
        /// </summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("{id:guid}/participants")]
        public async Task<IActionResult> AddParticipant(
            Guid id, [FromBody] AddParticipantRequest req)
        {
            await _tickets.AddParticipantAsync(id, req.UserId, req.Role, UserId);
            return NoContent();
        }

        /// <summary>
        /// Удалить участника из тикета.
        /// Только DepartmentHead своего отдела или Admin.
        /// </summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpDelete("{id:guid}/participants/{userId:guid}")]
        public async Task<IActionResult> RemoveParticipant(Guid id, Guid userId)
        {
            await _tickets.RemoveParticipantAsync(id, userId, UserId);
            return NoContent();
        }
    }
}
