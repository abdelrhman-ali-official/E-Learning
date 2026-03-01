namespace Shared.VideoModels;

public record CreateCourseVideoDTO
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string VideoId { get; init; } = string.Empty;
    public int Duration { get; init; }
    public int OrderIndex { get; init; }
    public bool IsPreview { get; init; }
}
