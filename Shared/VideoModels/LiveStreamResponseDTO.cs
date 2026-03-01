namespace Shared.VideoModels;

public record LiveStreamResponseDTO
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsLive { get; init; }
    public string EmbedUrl { get; init; } = string.Empty;
    public DateTime? ScheduledEnd { get; init; }
}
