using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Repositories
{
    public interface IPayrollRepository : IRepository<PayrollRecord>
    {
        Task<PayrollRecord?> GetByUserAndPeriodAsync(Guid userId, DateTime periodStart, DateTime periodEnd);
        Task<List<PayrollRecord>> GetByDepartmentAsync(Guid departmentId, int year, int month);
        Task UpdateStatusAsync(Guid recordId, string status);
    }
}
