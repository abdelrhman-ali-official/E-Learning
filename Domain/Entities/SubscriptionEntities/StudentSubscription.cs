using Domain.Entities.SecurityEntities;

namespace Domain.Entities.SubscriptionEntities
{
    public class StudentSubscription : BaseEntity<Guid>
    {
        public string StudentId { get; set; } = string.Empty;
        public Guid PackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public decimal FinalPrice { get; set; }
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.PendingPayment;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? DiscountCouponId { get; set; }

        public User Student { get; set; } = null!;
        public Package Package { get; set; } = null!;
        public DiscountCoupon? DiscountCoupon { get; set; }
        public ICollection<PaymentRequest> PaymentRequests { get; set; } = new List<PaymentRequest>();
    }
}
