using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Services.BackgroundServices
{
    public class PurchaseExpirationBackgroundService : BackgroundService
    {
        private readonly ILogger<PurchaseExpirationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Run daily

        public PurchaseExpirationBackgroundService(
            ILogger<PurchaseExpirationBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Purchase Expiration Background Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Running purchase expiration check at: {Time}", DateTime.UtcNow);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var purchaseService = scope.ServiceProvider.GetRequiredService<IServiceManager>()
                            .PurchaseService;

                        await purchaseService.DeactivateExpiredPurchasesAsync();
                    }

                    _logger.LogInformation("Purchase expiration check completed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while deactivating expired purchases");
                }

                // Wait for the next interval
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Purchase Expiration Background Service is stopping");
        }
    }
}
