using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<PayrollRecord> CalculateForUserAsync(Guid uderId, int year, int month);
        Task<List<PayrollRecord>> CalculateForDepartmentAsync(Guid departmentId, int year, int month);
        Task<PayrollRecord?> GetRecordAsync(Guid userId, int year, int month);
        Task<List<PayrollRecord>> GetUserRecordsAsync(Guid userId);
        Task<List<PayrollRecord>> GetDepartmentRecordsAsync(Guid departmentId, int year, int month);
        Task ApproveRecordAsync(Guid recordId);
        Task MarkAsPaidAsync(Guid recordId);
        Task<decimal> GetTotalDepartmentSalaryAsync(Guid departmentId, int year, int month);
    }
}
