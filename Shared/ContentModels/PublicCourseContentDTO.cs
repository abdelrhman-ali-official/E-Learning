namespace Shared.ContentModels
{
   
    public record PublicCourseContentDTO
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ThumbnailUrl { get; init; }
        public bool IsDownloadable { get; init; }
    }
}
