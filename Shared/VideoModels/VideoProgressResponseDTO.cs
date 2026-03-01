namespace Shared.VideoModels;

public record VideoProgressResponseDTO
{
    public Guid Id { get; init; }
    public Guid VideoId { get; init; }
    public int WatchedSeconds { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime LastUpdated { get; init; }
}
