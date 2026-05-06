using BetterCRM.Business.Exceptions;
using BetterCRM.Business.Policies;
using BetterCRM.Core.Extensions;
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
        private readonly ITicketRepository _ticketRepo; // ✅ НОВОЕ

        public PayrollService(
            IShiftRepository shiftRepo, IWorkSessionRepository sessionRepo,
            IUserRepository userRepo, IPayrollRepository payrollRepo,
            ITicketRepository ticketRepo)
        {
            _shiftRepo = shiftRepo; _sessionRepo = sessionRepo;
            _userRepo = userRepo; _payrollRepo = payrollRepo;
            _ticketRepo = ticketRepo;
        }

        public async Task<PayrollRecord> CalculateForUserAsync(Guid userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new NotFoundException("Пользователь не найден");

            var scheduled = await _shiftRepo.GetTotalScheduledHoursAsync(userId, start, end);
            var actualRaw = await _sessionRepo.GetTotalHoursAsync(userId, start, end);
            var actualBillable = RoundingPolicy.RoundDownHour(actualRaw);

            // ✅ ИЗМЕНЕНО: штрафы считаются раздельно
            var attendancePenalty = await _shiftRepo.GetTotalAttendancePenaltyAsync(userId, start, end);
            var ticketPenalty = await _payrollRepo.GetTicketPenaltyHoursAsync(userId, start, end);

            var (record, err) = PayrollRecord.Create(
                user.OrganizationId, userId, start, end,
                scheduled, actualBillable,
                attendancePenalty, ticketPenalty,
                user.Position.HourlyRate);
            if (err != null) throw new DomainException(err);

            // ✅ ИСПРАВЛЕНО: сначала создаём, потом удаляем старую (Upsert-паттерн)
            var existing = await _payrollRepo.GetByUserAndPeriodAsync(userId, start, end);
            if (existing != null) await _payrollRepo.DeleteAsync(existing.Id);

            return await _payrollRepo.AddAsync(record);
        }

        public async Task<List<PayrollRecord>> CalculateForDepartmentAsync(Guid departmentId, int year, int month)
        {
            var users = await _userRepo.GetActiveByDepartmentAsync(departmentId);
            var records = new List<PayrollRecord>();
            foreach (var u in users)
                records.Add(await CalculateForUserAsync(u.Id, year, month));
            return records;
        }

        public async Task<PayrollRecord?> GetRecordAsync(Guid userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            return await _payrollRepo.GetByUserAndPeriodAsync(userId, start, start.AddMonths(1).AddDays(-1));
        }

        // ✅ НОВОЕ: предпросмотр без сохранения (для дашборда)
        public async Task<PayrollPreviewDto> GetPreviewAsync(Guid userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            var user = await _userRepo.GetByIdAsync(userId) ?? throw new NotFoundException("Пользователь не найден");
            var scheduled = await _shiftRepo.GetTotalScheduledHoursAsync(userId, start, end);
            var actualBillable = RoundingPolicy.RoundDownHour(await _sessionRepo.GetTotalHoursAsync(userId, start, end));
            var attendancePenalty = await _shiftRepo.GetTotalAttendancePenaltyAsync(userId, start, end);
            var ticketPenalty = await _payrollRepo.GetTicketPenaltyHoursAsync(userId, start, end);
            var totalPenalty = attendancePenalty + ticketPenalty;
            var finalBillable = Math.Max(0, actualBillable - totalPenalty);

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
                HourlyRate = user.Position.HourlyRate,
                EstimatedSalary = Math.Round(finalBillable * user.Position.HourlyRate, 2)
            };
        }

        public async Task ApproveAsync(Guid recordId) =>
            await _payrollRepo.UpdateStatusAsync(recordId, PayrollStatus.Approved.ToString());

        public async Task MarkAsPaidAsync(Guid recordId) =>
            await _payrollRepo.UpdateStatusAsync(recordId, PayrollStatus.Paid.ToString());

        public Task<PayrollRecord> PreviewForUserAsync(Guid userId, int year, int month)
        {
            throw new NotImplementedException();
        }
    }

    public record PayrollPreviewDto
    {
        public Guid UserId { get; init; }
        public string Period { get; init; } = string.Empty;
        public decimal ScheduledHours { get; init; }
        public decimal ActualHours { get; init; }
        public decimal AttendancePenaltyHours { get; init; }
        public decimal TicketPenaltyHours { get; init; }
        public decimal TotalPenaltyHours { get; init; }
        public decimal FinalBillableHours { get; init; }
        public decimal HourlyRate { get; init; }
        public decimal EstimatedSalary { get; init; }
    }
}