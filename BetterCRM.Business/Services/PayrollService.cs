using BetterCRM.Business.Exceptions;
using BetterCRM.Business.Policies;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using Microsoft.EntityFrameworkCore;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IShiftRepository _shiftRepo;
        private readonly IWorkSessionRepository _sessionRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPayrollRepository _payrollRepo;

        public PayrollService(
            IShiftRepository shiftRepo,
            IWorkSessionRepository sessionRepo,
            IUserRepository userRepo,
            IPayrollRepository payrollRepo)
        {
            _shiftRepo = shiftRepo;
            _sessionRepo = sessionRepo;
            _userRepo = userRepo;
            _payrollRepo = payrollRepo;
        }

        public async Task<PayrollRecord> CalculateForUserAsync(Guid userId, int year, int month)
        {
            var (start, end) = GetPeriod(year, month);

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new NotFoundException("Пользователь не найден");

            var scheduled = await _shiftRepo.GetTotalScheduledHoursAsync(userId, start, end);
            var actualRaw = await _sessionRepo.GetTotalHoursAsync(userId, start, end);
            var actualBillable = RoundingPolicy.RoundDownHour(actualRaw);
            var attendancePenalty = await _shiftRepo.GetTotalAttendancePenaltyAsync(userId, start, end);
            var ticketPenalty = await _payrollRepo.GetTicketPenaltyHoursAsync(userId, start, end);

            var (record, err) = PayrollRecord.Create(
                user.OrganizationId, userId, start, end,
                scheduled, actualBillable,
                attendancePenalty, ticketPenalty,
                user.Position.HourlyRate);
            if (err != null) throw new DomainException(err);

            await _payrollRepo.UpsertAsync(record);
            return record;
        }

        public async Task<List<PayrollRecord>> CalculateForDepartmentAsync(
            Guid departmentId, int year, int month)
        {
            var users = await _userRepo.GetActiveByDepartmentAsync(departmentId);

            var tasks = users.Select(u => CalculateForUserAsync(u.Id, year, month));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public async Task<PayrollRecord?> GetRecordAsync(Guid userId, int year, int month)
        {
            var (start, end) = GetPeriod(year, month);
            return await _payrollRepo.GetByUserAndPeriodAsync(userId, start, end);
        }
        public async Task<PayrollPreviewDto> PreviewForUserAsync(Guid userId, int year, int month)
        {
            var (start, end) = GetPeriod(year, month);

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new NotFoundException("Пользователь не найден");

            var scheduled = await _shiftRepo.GetTotalScheduledHoursAsync(userId, start, end);
            var actualBillable = RoundingPolicy.RoundDownHour(
                                        await _sessionRepo.GetTotalHoursAsync(userId, start, end));
            var attendancePenalty = await _shiftRepo.GetTotalAttendancePenaltyAsync(userId, start, end);
            var ticketPenalty = await _payrollRepo.GetTicketPenaltyHoursAsync(userId, start, end);
            var totalPenalty = attendancePenalty + ticketPenalty;
            var finalBillable = Math.Max(0, actualBillable - totalPenalty);
            var rate = user.Position.HourlyRate;

            return new PayrollPreviewDto
            {
                UserId = userId,
                Period = $"{year}-{month:D2}",
                ScheduledHours = scheduled,
                ActualHours = actualBillable,
                AttendancePenaltyHours = attendancePenalty,
                TicketPenaltyHours = ticketPenalty,
                TotalPenaltyHours = totalPenalty,
                FinalBillableHours = finalBillable,
                HourlyRate = rate,
                EstimatedSalary = Math.Round(finalBillable * rate, 2)
            };
        }

        public async Task ApproveAsync(Guid recordId) =>
            await _payrollRepo.UpdateStatusAsync(recordId, "Approved");

        public async Task MarkAsPaidAsync(Guid recordId) =>
            await _payrollRepo.UpdateStatusAsync(recordId, "Paid");

        private static (DateTime start, DateTime end) GetPeriod(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            return (start, start.AddMonths(1).AddDays(-1));
        }
    }

   
}