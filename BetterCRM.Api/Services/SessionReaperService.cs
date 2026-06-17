using BetterCRM.Core.Interfaces.Services;

namespace BetterCRM.Api.Services
{
    public class SessionReaperService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SessionReaperService> _logger;

        public SessionReaperService(IServiceScopeFactory scopeFactory, ILogger<SessionReaperService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var tracking = scope.ServiceProvider.GetRequiredService<ITimeTrackingService>();
                    var closed = await tracking.AutoCloseExpiredSessionsAsync();
                    if (closed > 0)
                        _logger.LogInformation("Автоматически завершено сессий: {Count}", closed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при автозавершении просроченных сессий");
                }
            }
        }
    }
}
