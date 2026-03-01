namespace Domain.Entities.ContentEntities
{
    public class ManualPaymentRequest : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public int ContentId { get; set; }
        public PaymentMethod TransferMethod { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        public string ScreenshotUrl { get; set; } = string.Empty;
        public ManualPaymentStatus Status { get; set; } = ManualPaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }
        public string? RejectionReason { get; set; }
        public decimal Amount { get; set; }

        public Content Content { get; set; } = null!;
    }

    public enum PaymentMethod
    {
        InstaPay = 1,
        VodafoneCash = 2
    }

    public enum ManualPaymentStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
