using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Services.BackgroundServices
{
    public class SubscriptionExpirationBackgroundService : BackgroundService
    {
        private readonly ILogger<SubscriptionExpirationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily

        public SubscriptionExpirationBackgroundService(
            ILogger<SubscriptionExpirationBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Expiration Background Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Running subscription expiration check at: {Time}", DateTime.UtcNow);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var subscriptionService = scope.ServiceProvider.GetRequiredService<IServiceManager>()
                            .SubscriptionService;

                        await subscriptionService.ExpireSubscriptionsAsync(stoppingToken);
                    }

                    _logger.LogInformation("Subscription expiration check completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while expiring subscriptions");
                }

                // Wait for the next interval
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Subscription Expiration Background Service is stopping");
        }
    }
}
