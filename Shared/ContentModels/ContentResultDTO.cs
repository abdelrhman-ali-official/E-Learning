namespace Shared.ContentModels
{
    public record ContentResultDTO
    {
        public int Id { get; init; }
        public Guid CourseId { get; init; }
        public string InstructorId { get; init; } = string.Empty;
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? ThumbnailUrl { get; init; }
        public string Type { get; init; } = string.Empty;
        public string? ExternalVideoUrl { get; init; }
        public string? YoutubeVideoId { get; init; }
        public bool IsVisible { get; init; }
        public bool IsDownloadable { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
