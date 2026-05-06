using BetterCRM.Core.Models;

namespace BetterCRM.Core.Interfaces.Services
{
    public interface IPayrollService
    {
        Task<PayrollRecord> CalculateForUserAsync(Guid userId, int year, int month);
        Task<List<PayrollRecord>> CalculateForDepartmentAsync(Guid departmentId, int year, int month);
        Task<PayrollRecord?> GetRecordAsync(Guid userId, int year, int month);
        // Предварительный расчёт без сохранения — для отображения текущего заработка
        Task<PayrollRecord> PreviewForUserAsync(Guid userId, int year, int month);
    }
}
