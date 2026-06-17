using BetterCRM.Core.Constants;
using BetterCRM.Core.Extensions;
using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    public record CreateShiftRequest(Guid UserId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime);
    public record UpdateShiftRequest(TimeSpan? StartTime, TimeSpan? EndTime, ShiftStatus? Status);
    public record AddBreakRequest(TimeSpan StartTime, TimeSpan EndTime, BreakType Type, bool IsPaid);

    [Authorize]
    public class ShiftsController : BaseApiController
    {
        private readonly IShiftService _shifts;

        public ShiftsController(IShiftService shifts, ICurrentUserProvider up)
            : base(up) => _shifts = shifts;


        [Authorize(Roles = Roles.Managers)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShiftRequest req)
        {
            var shift = await _shifts.CreateAsync(
                new CreateShiftCommand(req.UserId, req.Date, req.StartTime, req.EndTime),
                UserId, UserRole, UserDeptId);
            return CreatedAtAction(nameof(GetForUser), new { userId = req.UserId }, shift);
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var shift = await _shifts.GetTodayShiftAsync(UserId);
            if (shift == null) return Ok(new { hasShift = false });
            return Ok(new { hasShift = true, shift });
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetForUser(
            Guid userId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (UserRole == "Employee" && userId != UserId)
                return Forbid();

            var shifts = await _shifts.GetForUserAsync(userId, from, to);
            return Ok(shifts);
        }

        [Authorize(Roles = Roles.Managers)]
        [HttpGet("department/{departmentId:guid}")]
        public async Task<IActionResult> GetForDepartment(
            Guid departmentId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (UserRole == Roles.DepartmentHead && departmentId != UserDeptId)
                return Forbid();

            var shifts = await _shifts.GetForDepartmentAsync(departmentId, from, to);
            return Ok(shifts);
        }

        [Authorize(Roles = "Admin,OrganizationHead")]
        [HttpGet("organization/{orgId:guid}")]
        public async Task<IActionResult> GetForOrganization(
            Guid orgId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var shifts = await _shifts.GetForOrganizationAsync(orgId, from, to);
            return Ok(shifts);
        }

        [Authorize(Roles = Roles.Managers)]
        [HttpPatch("{shiftId:guid}")]
        public async Task<IActionResult> Update(
            Guid shiftId, [FromBody] UpdateShiftRequest req)
        {
            await _shifts.UpdateAsync(
                shiftId,
                new UpdateShiftCommand(req.StartTime, req.EndTime, req.Status),
                UserId, UserRole, UserDeptId);
            return NoContent();
        }

        [Authorize(Roles = Roles.Managers)]
        [HttpPost("{shiftId:guid}/breaks")]
        public async Task<IActionResult> AddBreak(Guid shiftId, [FromBody] AddBreakRequest req)
        {
            var shiftBreak = await _shifts.AddBreakAsync(
                shiftId,
                new AddBreakCommand(req.StartTime, req.EndTime, req.Type, req.IsPaid),
                UserRole, UserDeptId);
            return Ok(shiftBreak);
        }

        [Authorize(Roles = Roles.Managers)]
        [HttpDelete("breaks/{breakId:guid}")]
        public async Task<IActionResult> RemoveBreak(Guid breakId)
        {
            await _shifts.RemoveBreakAsync(breakId, UserRole, UserDeptId);
            return NoContent();
        }
    }
}
