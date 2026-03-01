namespace Domain.Entities.SubscriptionEntities
{
    public class PaymentRequest : BaseEntity<Guid>
    {
        public Guid StudentSubscriptionId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string? PaymentProofUrl { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? AdminNotes { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }

        public StudentSubscription StudentSubscription { get; set; } = null!;
    }
}
