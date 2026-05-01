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
        private readonly ITimeLogRepository _timeLogRepo;
        private readonly IUserRepository _userRepo;

        public TimeTrackingService(IWorkSessionRepository sessionRepo, ITimeLogRepository timeLogRepo, IUserRepository userRepo)
        {
            _sessionRepo = sessionRepo;
            _timeLogRepo = timeLogRepo;
            _userRepo = userRepo;
        }

        public async Task<WorkSession> StartSessionAsync(StartSessionCommand command)
        {
            var user = await _userRepo.GetByIdAsync(command.UserId)
                       ?? throw new NotFoundException("Пользователь не найден");

            if (await _sessionRepo.GetActiveSessionAsync(command.UserId) != null)
                throw new ConflictException("Активная сессия уже существует");

            var (session, err) = WorkSession.Start(user.OrganizationId, command.UserId, shiftId: null);
            if (err != null) throw new DomainException(err);

            return await _sessionRepo.AddAsync(session);
        }

        public async Task<decimal> StopSessionAsync(StopSessionCommand command)
        {
            var session = await _sessionRepo.GetActiveSessionAsync(command.UserId)
                          ?? throw new NotFoundException("Нет активной сессии");

            var (ok, err) = session.Stop(command.Description);
            if (!ok) throw new DomainException(err!);

            await _sessionRepo.UpdateAsync(session);
            return session.DurationHours;
        }

        public async Task<WorkSession?> GetActiveSessionAsync(Guid userId) =>
            await _sessionRepo.GetActiveSessionAsync(userId);

        public async Task<List<WorkSession>> GetUserSessionsAsync(Guid userId, DateTime? from = null, DateTime? to = null) =>
            await _sessionRepo.GetByUserAsync(userId, from, to);

        public async Task<decimal> GetTodayHoursAsync(Guid userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _sessionRepo.GetTotalHoursAsync(userId, today, today.AddDays(1));
        }

        public async Task<decimal> GetWeekHoursAsync(Guid userId)
        {
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(7);
            return await _sessionRepo.GetTotalHoursAsync(userId, startOfWeek, endOfWeek);
        }

        public async Task<decimal> GetMonthHoursAsync(Guid userId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            return await _sessionRepo.GetTotalHoursAsync(userId, start, end);
        }

        public async Task<Dictionary<DateTime, decimal>> GetHoursByDayAsync(Guid userId, DateTime from, DateTime to) =>
            await _sessionRepo.GetHoursByDayAsync(userId, from, to);

        public async Task<decimal> GetTotalHoursByTicketAsync(Guid ticketId) =>
            await _timeLogRepo.GetTotalHoursByTicketAsync(ticketId);
    }
}
    

