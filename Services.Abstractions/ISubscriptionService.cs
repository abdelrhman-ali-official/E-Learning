using Shared.SubscriptionModels;

namespace Services.Abstractions
{
    public interface ISubscriptionService
    {
        Task<SubscriptionResponseDTO> SubscribeAsync(Guid studentId, SubscribeRequestDTO dto, CancellationToken cancellationToken = default);
        Task<SubscriptionResponseDTO> GetSubscriptionByIdAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task<SubscriptionResponseDTO?> GetStudentActiveSubscriptionAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SubscriptionResponseDTO>> GetAllSubscriptionsAsync(CancellationToken cancellationToken = default);
        Task<decimal> CalculateFinalPriceAsync(Guid packageId, Domain.Entities.SubscriptionEntities.BillingCycle billingCycle, string? couponCode = null, CancellationToken cancellationToken = default);
        Task CancelSubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
        Task ExpireSubscriptionsAsync(CancellationToken cancellationToken = default);
    }
}
