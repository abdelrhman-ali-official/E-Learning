namespace Shared.VideoModels;

public record CourseVideoResponseDTO
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Duration { get; init; }
    public Guid CourseId { get; init; }
    public int OrderIndex { get; init; }
    public bool IsPreview { get; init; }
    public DateTime CreatedAt { get; init; }
}
