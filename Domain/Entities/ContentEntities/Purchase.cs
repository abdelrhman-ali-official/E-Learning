namespace Domain.Entities.ContentEntities
{
    public class Purchase : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public int ContentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int? ManualPaymentRequestId { get; set; }

        public Content Content { get; set; } = null!;
        public ManualPaymentRequest? ManualPaymentRequest { get; set; }
    }
}
