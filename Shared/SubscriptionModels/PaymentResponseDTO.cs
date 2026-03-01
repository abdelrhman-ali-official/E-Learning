using Domain.Entities.SubscriptionEntities;

namespace Shared.SubscriptionModels
{
    public class PaymentResponseDTO
    {
        public Guid Id { get; set; }
        public Guid StudentSubscriptionId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string PaymentProofUrl { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedByEmail { get; set; }
    }
}
