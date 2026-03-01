namespace Shared.VideoModels;

public record LiveSessionResponseDTO
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string InstructorId { get; init; } = string.Empty;
    public string? YouTubeLiveVideoId { get; init; }
    public Guid CourseId { get; init; }
    public string? RoomName { get; init; }
    public string Provider { get; init; } = "JITSI";
    public DateTime ScheduledStart { get; init; }
    public DateTime ScheduledEnd { get; init; }
    public bool IsActive { get; init; }
    public bool IsRecordedAvailable { get; init; }
    public string? RecordingLink { get; init; }
    public bool IsLive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
