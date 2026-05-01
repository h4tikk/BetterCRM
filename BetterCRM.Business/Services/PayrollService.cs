using BetterCRM.Business.Exceptions;
using BetterCRM.Business.Policies;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;

using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IShiftRepository _shiftRepo;
        private readonly IWorkSessionRepository _sessionRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPayrollRepository _payrollRepo;

        public PayrollService(IShiftRepository shiftRepo, IWorkSessionRepository sessionRepo, IUserRepository userRepo, IPayrollRepository payrollRepo)
            => (_shiftRepo, _sessionRepo, _userRepo, _payrollRepo) = (shiftRepo, sessionRepo, userRepo, payrollRepo);

        public async Task<PayrollRecord> CalculateForUserAsync(Guid userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var user = await _userRepo.GetByIdAsync(userId) ?? throw new NotFoundException("Пользователь не найден");
            var scheduled = await _shiftRepo.GetTotalScheduledHoursAsync(userId, start, end);
            var actualRaw = await _sessionRepo.GetTotalHoursAsync(userId, start, end);

            var actualBillable = RoundingPolicy.RoundDownHour(actualRaw);
            var penalty = Math.Max(0, scheduled - actualBillable);

            var existing = await _payrollRepo.GetByUserAndPeriodAsync(userId, start, end);
            if (existing != null) await _payrollRepo.DeleteAsync(existing.Id);

            var (record, err) = PayrollRecord.Create(
                user.OrganizationId, userId, start, end, scheduled, actualRaw, penalty, actualBillable, user.Position.HourlyRate);
            if (err != null) throw new DomainException(err);

            return await _payrollRepo.AddAsync(record);
        }

        public async Task<List<PayrollRecord>> CalculateForDepartmentAsync(Guid departmentId, int year, int month)
        {
            var users = await _userRepo.GetActiveByDepartmentAsync(departmentId);
            var records = new List<PayrollRecord>();
            foreach (var u in users) records.Add(await CalculateForUserAsync(u.Id, year, month));
            return records;
        }

        public async Task<PayrollRecord?> GetRecordAsync(Guid userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            return await _payrollRepo.GetByUserAndPeriodAsync(userId, start, start.AddMonths(1).AddDays(-1));
        }
    }
}
