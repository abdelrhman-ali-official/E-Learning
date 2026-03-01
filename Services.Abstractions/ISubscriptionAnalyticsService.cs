using Shared.SubscriptionModels;

namespace Services.Abstractions
{
    public interface ISubscriptionAnalyticsService
    {
        Task<SubscriptionAnalyticsDTO> GetAnalyticsAsync(CancellationToken cancellationToken = default);
    }
}
