namespace Shared.ContentModels
{
    public class StreamResponseDTO
    {
        public string StreamUrl { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}
