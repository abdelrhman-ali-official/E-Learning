namespace Shared.ContentModels
{
    public record PurchaseResultDTO
    {
        public int Id { get; init; }
        public string UserId { get; init; } = string.Empty;
        public int ContentId { get; init; }
        public string ContentTitle { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public DateTime PurchaseDate { get; init; }
        public DateTime ExpiryDate { get; init; }
        public bool IsActive { get; init; }
    }
}
