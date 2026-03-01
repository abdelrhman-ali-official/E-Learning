namespace Shared.ContentModels
{
    public record ContentAccessDTO
    {
        public int ContentId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string? AccessUrl { get; init; }
        public string? YoutubeEmbedUrl { get; init; }
        public DateTime ExpiryDate { get; init; }
        public bool IsLiveActive { get; init; }
    }
}
