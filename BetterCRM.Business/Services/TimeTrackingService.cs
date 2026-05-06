using BetterCRM.Business.Exceptions;
using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Interfaces.Services;
using BetterCRM.Core.Models;
using static BetterCRM.Business.Exceptions.DomainException;

namespace BetterCRM.Business.Services
{
    public class TimeTrackingService : ITimeTrackingService
    {
        private readonly IWorkSessionRepository _sessionRepo;
        private readonly IShiftRepository _shiftRepo;
        private readonly ITimeLogRepository _timeLogRepo;
        private readonly IUserRepository _userRepo;
        private readonly IPayrollRepository _payrollRepo;

        public TimeTrackingService(
            IWorkSessionRepository sessionRepo,
            IShiftRepository shiftRepo,
            ITimeLogRepository timeLogRepo,
            IUserRepository userRepo,
            IPayrollRepository payrollRepo)
        {
            _sessionRepo = sessionRepo;
            _shiftRepo = shiftRepo;
            _timeLogRepo = timeLogRepo;
            _userRepo = userRepo;
            _payrollRepo = payrollRepo;
        }

        public async Task<WorkSession> StartSessionAsync(StartSessionCommand command)
        {
            var user = await _userRepo.GetByIdAsync(command.UserId)
                ?? throw new NotFoundException("Пользователь не найден");

            if (await _sessionRepo.GetActiveSessionAsync(command.UserId) != null)
                throw new ConflictException("Активная сессия уже существует");

            // Привязываем к смене на сегодня если есть
            var todayShift = await _shiftRepo.GetByUserAndDateAsync(command.UserId, DateTime.UtcNow.Date);

            var (session, err) = WorkSession.Start(user.OrganizationId, command.UserId, todayShift?.Id);
            if (err != null) throw new DomainException(err);

            return await _sessionRepo.AddAsync(session);
        }

        // ✅ Возвращает decimal (DurationHours) как в интерфейсе
        public async Task<decimal> StopSessionAsync(StopSessionCommand command)
        {
            var session = await _sessionRepo.GetActiveSessionAsync(command.UserId)
                ?? throw new NotFoundException("Нет активной сессии");

            var (ok, err) = session.Stop(command.Description);
            if (!ok) throw new DomainException(err!);

            // Начисляем штраф посещаемости и завершаем смену
            if (session.ShiftId.HasValue)
            {
                var shift = await _shiftRepo.GetByIdAsync(session.ShiftId.Value);
                if (shift != null)
                {
                    var latenessHours = CalculateLatenessHours(shift, session.StartedAt);
                    var earlyLeaveHours = CalculateEarlyLeaveHours(shift, session.EndedAt!.Value);

                    if (latenessHours > 0 || earlyLeaveHours > 0)
                        shift.ApplyAttendancePenalty(latenessHours, earlyLeaveHours);

                    shift.Complete();
                    await _shiftRepo.UpdateAsync(shift);
                }
            }

            await _sessionRepo.UpdateAsync(session);
            return session.DurationHours;
        }

        public async Task<WorkSession?> GetActiveSessionAsync(Guid userId) =>
            await _sessionRepo.GetActiveSessionAsync(userId);

        public async Task<List<WorkSession>> GetUserSessionsAsync(
            Guid userId, DateTime? from = null, DateTime? to = null) =>
            await _sessionRepo.GetByUserAsync(userId, from, to);

        public async Task<decimal> GetTodayHoursAsync(Guid userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _sessionRepo.GetTotalHoursAsync(userId, today, today.AddDays(1));
        }

        public async Task<decimal> GetWeekHoursAsync(Guid userId)
        {
            var (start, _) = GetCurrentWeekRange();
            return await _sessionRepo.GetTotalHoursAsync(userId, start, start.AddDays(7));
        }

        public async Task<decimal> GetMonthHoursAsync(Guid userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            return await _sessionRepo.GetTotalHoursAsync(userId, start, start.AddMonths(1));
        }

        public async Task<Dictionary<DateTime, decimal>> GetHoursByDayAsync(
            Guid userId, DateTime from, DateTime to) =>
            await _sessionRepo.GetHoursByDayAsync(userId, from, to);

        public async Task<decimal> GetTotalHoursByTicketAsync(Guid ticketId) =>
            await _timeLogRepo.GetTotalHoursByTicketAsync(ticketId);

        // ✅ Возвращает WeekEarningsDto строго по полям из интерфейса
        public async Task<WeekEarningsDto> GetWeekEarningsAsync(Guid userId)
        {
            var (weekStart, weekEnd) = GetCurrentWeekRange();

            var user = await _userRepo.GetByIdAsync(userId)
                ?? throw new NotFoundException("Пользователь не найден");

            var workedHours = await _sessionRepo.GetTotalHoursAsync(userId, weekStart, weekEnd);
            var attendancePenalty = await _shiftRepo.GetTotalAttendancePenaltyAsync(userId, weekStart, weekEnd);
            var ticketPenalty = await _payrollRepo.GetTicketPenaltyHoursAsync(userId, weekStart, weekEnd);
            var penaltyHours = attendancePenalty + ticketPenalty;
            var rate = user.Position.HourlyRate;

            // BillableHours — сколько часов идёт в оплату (не больше фактических)
            var billableHours = Math.Max(0, workedHours - penaltyHours);

            // CurrentEarnings — до вычета штрафов
            var currentEarnings = Math.Round(workedHours * rate, 2);

            // EstimatedNet — с учётом штрафов
            var estimatedNet = Math.Round(billableHours * rate, 2);

            return new WeekEarningsDto(
                WorkedHours: workedHours,
                BillableHours: billableHours,
                HourlyRate: rate,
                CurrentEarnings: currentEarnings,
                PenaltyHours: penaltyHours,
                EstimatedNet: estimatedNet
            );
        }

        // ── Вспомогательные методы ────────────────────────────────────────────

        private static (DateTime start, DateTime end) GetCurrentWeekRange()
        {
            var today = DateTime.UtcNow.Date;
            // Неделя с понедельника
            var start = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            return (start, start.AddDays(7));
        }

        private static decimal CalculateLatenessHours(Shift shift, DateTime actualStart)
        {
            var scheduled = shift.Date.Add(shift.StartTime);
            var diff = (actualStart - scheduled).TotalHours;
            return diff > 0 ? Math.Round((decimal)diff, 2) : 0;
        }

        private static decimal CalculateEarlyLeaveHours(Shift shift, DateTime actualEnd)
        {
            var scheduled = shift.Date.Add(shift.EndTime);
            var diff = (scheduled - actualEnd).TotalHours;
            return diff > 0 ? Math.Round((decimal)diff, 2) : 0;
        }
    }
}