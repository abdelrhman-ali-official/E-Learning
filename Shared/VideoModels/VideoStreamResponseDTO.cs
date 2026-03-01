namespace Shared.VideoModels;

public record VideoStreamResponseDTO
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Duration { get; init; }
    public string EmbedUrl { get; init; } = string.Empty;
    public bool IsPreview { get; init; }
}
