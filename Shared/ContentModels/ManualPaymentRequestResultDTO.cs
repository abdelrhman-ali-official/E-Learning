namespace Shared.ContentModels
{
    public record ManualPaymentRequestResultDTO
    {
        public int Id { get; init; }
        public string UserId { get; init; } = string.Empty;
        public int ContentId { get; init; }
        public string ContentTitle { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public string TransferMethod { get; init; } = string.Empty;
        public string ReferenceNumber { get; init; } = string.Empty;
        public string ScreenshotUrl { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? ReviewedAt { get; init; }
        public string? ReviewedBy { get; init; }
        public string? RejectionReason { get; init; }
    }
}
