using BetterCRM.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BetterCRM.Api.Controllers
{
    [Authorize]
    public class PayrollController : BaseApiController
    {
        private readonly IPayrollService _payroll;

        public PayrollController(IPayrollService payroll, ICurrentUserProvider up)
            : base(up) => _payroll = payroll;

        [HttpGet("preview")]
        public async Task<IActionResult> Preview(
            [FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var record = await _payroll.PreviewForUserAsync(
                UserId, year ?? now.Year, month ?? now.Month);
            return Ok(record);
        }

        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpGet("preview/user/{userId:guid}")]
        public async Task<IActionResult> PreviewForUser(
            Guid userId,
            [FromQuery] int? year, [FromQuery] int? month)
        {
            var now = DateTime.UtcNow;
            var record = await _payroll.PreviewForUserAsync(
                userId, year ?? now.Year, month ?? now.Month);
            return Ok(record);
        }

        [Authorize(Roles = "Admin,OrganizationHead")]
        [HttpPost("calculate/user/{userId:guid}")]
        public async Task<IActionResult> CalculateForUser(
            Guid userId,
            [FromQuery] int year, [FromQuery] int month)
        {
            var record = await _payroll.CalculateForUserAsync(userId, year, month);
            return Ok(record);
        }


        [Authorize(Roles = "Admin,OrganizationHead,DepartmentHead")]
        [HttpPost("calculate/department/{departmentId:guid}")]
        public async Task<IActionResult> CalculateForDepartment(
            Guid departmentId,
            [FromQuery] int year, [FromQuery] int month)
        {
            if (UserRole == "DepartmentHead" && departmentId != UserDeptId)
                return Forbid();
            var records = await _payroll.CalculateForDepartmentAsync(departmentId, year, month);
            return Ok(records);
        }


        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetRecord(
            Guid userId,
            [FromQuery] int year, [FromQuery] int month)
        {
            if (UserRole == "Employee" && userId != UserId)
                return Forbid();

            var record = await _payroll.GetRecordAsync(userId, year, month);
            if (record == null)
                return NotFound(new { error = "Расчёт не найден. Используйте /preview для предварительного просмотра" });
            return Ok(record);
        }
    }
}
