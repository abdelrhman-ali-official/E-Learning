using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Start.BackgroundServices;


public class LiveSessionExpirationBackgroundService : BackgroundService
{
    private readonly ILogger<LiveSessionExpirationBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);

    public LiveSessionExpirationBackgroundService(
        ILogger<LiveSessionExpirationBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Live Session Expiration Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Checking for expired live sessions at {Time}", DateTime.UtcNow);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();
                    await serviceManager.LiveSessionService.DeactivateExpiredSessionsAsync();
                }

                _logger.LogInformation("Expired live sessions check completed at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating expired live sessions");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }

        _logger.LogInformation("Live Session Expiration Background Service stopped");
    }
}
