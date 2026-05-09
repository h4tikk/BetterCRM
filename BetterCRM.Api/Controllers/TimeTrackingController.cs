using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    public record StopSessionRequest(string? Description);

    [Authorize]
    public class TimeTrackingController : BaseApiController
    {
        private readonly ITimeTrackingService _tracking;

        public TimeTrackingController(ITimeTrackingService tracking, ICurrentUserProvider up)
            : base(up) => _tracking = tracking;

        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            var session = await _tracking.StartSessionAsync(new StartSessionCommand(UserId));
            return CreatedAtAction(nameof(GetActive), session);
        }

        [HttpPost("stop")]
        public async Task<IActionResult> Stop([FromBody] StopSessionRequest req)
        {
            var hours = await _tracking.StopSessionAsync(new StopSessionCommand(UserId, req.Description));
            return Ok(new { hoursWorked = hours });
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var session = await _tracking.GetActiveSessionAsync(UserId);
            if (session == null) return Ok(new { active = false });
            return Ok(new { active = true, session });
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions(
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var sessions = await _tracking.GetUserSessionsAsync(UserId, from, to);
            return Ok(sessions);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var today = await _tracking.GetTodayHoursAsync(UserId);
            var week = await _tracking.GetWeekHoursAsync(UserId);
            var now = DateTime.UtcNow;
            var month = await _tracking.GetMonthHoursAsync(UserId, now.Year, now.Month);
            return Ok(new { today, week, month });
        }

        [HttpGet("earnings/week")]
        public async Task<IActionResult> GetWeekEarnings()
        {
            var earnings = await _tracking.GetWeekEarningsAsync(UserId);
            return Ok(earnings);
        }

        [HttpGet("hours-by-day")]
        public async Task<IActionResult> GetHoursByDay(
            [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var data = await _tracking.GetHoursByDayAsync(UserId, from, to);
            return Ok(data);
        }
    }
}
