using BetterCRM.Core.Extensions;
using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    public record CreateShiftRequest(Guid UserId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime);
    public record UpdateShiftRequest(TimeSpan? StartTime, TimeSpan? EndTime, ShiftStatus? Status);

    [Authorize]
    public class ShiftsController : BaseApiController
    {
        private readonly IShiftService _shifts;

        public ShiftsController(IShiftService shifts, ICurrentUserProvider up)
            : base(up) => _shifts = shifts;

        /// <summary>
        /// Создать смену сотруднику.
        /// DepartmentHead — только своему отделу (Employee).
        /// OrganizationHead — любому в своей организации (включая DepartmentHead).
        /// Admin — всем без ограничений.
        /// </summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShiftRequest req)
        {
            var shift = await _shifts.CreateAsync(
                new CreateShiftCommand(req.UserId, req.Date, req.StartTime, req.EndTime),
                UserId, UserRole, UserDeptId);
            return CreatedAtAction(nameof(GetForUser), new { userId = req.UserId }, shift);
        }

        /// <summary>Смена текущего пользователя на сегодня (для кнопки «Начать смену»)</summary>
        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var shift = await _shifts.GetTodayShiftAsync(UserId);
            if (shift == null) return Ok(new { hasShift = false });
            return Ok(new { hasShift = true, shift });
        }

        /// <summary>Смены конкретного пользователя за период</summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetForUser(
            Guid userId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            // Employee может смотреть только свои смены
            if (UserRole == "Employee" && userId != UserId)
                return Forbid();

            var shifts = await _shifts.GetForUserAsync(userId, from, to);
            return Ok(shifts);
        }

        /// <summary>
        /// Смены отдела за период.
        /// DepartmentHead — только своего отдела.
        /// </summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpGet("department/{departmentId:guid}")]
        public async Task<IActionResult> GetForDepartment(
            Guid departmentId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (UserRole == "DepartmentHead" && departmentId != UserDeptId)
                return Forbid();

            var shifts = await _shifts.GetForDepartmentAsync(departmentId, from, to);
            return Ok(shifts);
        }

        /// <summary>
        /// Расписание всей организации (только OrganizationHead и Admin).
        /// </summary>
        [Authorize(Roles = "Admin,OrganizationHead")]
        [HttpGet("organization")]
        public async Task<IActionResult> GetForOrganization(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var shifts = await _shifts.GetForOrganizationAsync(from, to);
            return Ok(shifts);
        }

        /// <summary>Обновить/отменить смену</summary>
        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
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
    }
}
