using Domain.Entities.SubscriptionEntities;

namespace Shared.SubscriptionModels
{
    public class SubscriptionResponseDTO
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public decimal FinalPrice { get; set; }
        public SubscriptionStatus Status { get; set; }
        public string? CouponCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
