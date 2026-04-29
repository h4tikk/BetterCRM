using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<PayrollRecord> CalculateForUserAsync(Guid userId, int year, int month);
        Task<List<PayrollRecord>> CalculateForDepartmentAsync(Guid departmentId, int year, int month);
        Task<PayrollRecord?> GetRecordAsync(Guid userId, int year, int month);
    }
}
