using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IPayrollRepository : IRepository<PayrollRecord>
    {
        Task<PayrollRecord?> GetByUserAndPeriodAsync(Guid userId, DateTime from, DateTime to);
        Task<List<PayrollRecord>> GetByUserAsync(Guid userId);
        Task<List<PayrollRecord>> GetByDepartmentAsync(Guid departmentId, int year, int month);
        Task<List<PayrollRecord>> GetByStatusAsync(string status);
        Task<decimal> GetTotalSalaryAsync(Guid departmentId, int year, int month);
        Task<Dictionary<string, decimal>> GetSalaryByUserAsync(Guid departmentId, int year, int month);
        Task UpdateStatusAsync(Guid recordId, string newStatus);
    }
}
