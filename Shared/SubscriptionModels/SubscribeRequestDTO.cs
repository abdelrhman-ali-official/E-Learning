using Domain.Entities.SubscriptionEntities;

namespace Shared.SubscriptionModels
{
    public class SubscribeRequestDTO
    {
        public Guid PackageId { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public string? CouponCode { get; set; }
    }
}
