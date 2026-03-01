namespace Domain.Entities.VideoEntities;

/// <summary>
/// Tracks student progress for a specific video
/// </summary>
public class VideoProgress : BaseEntity<Guid>
{
    /// <summary>
    /// Foreign key to User (IdentityUser)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to CourseVideo
    /// </summary>
    public Guid VideoId { get; set; }

    /// <summary>
    /// Number of seconds watched
    /// </summary>
    public int WatchedSeconds { get; set; }

    /// <summary>
    /// Whether the video is marked as completed
    /// Auto-set to true if WatchedSeconds >= 90% of video duration
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Last time progress was updated
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Navigation property to CourseVideo
    /// </summary>
    public CourseVideo Video { get; set; } = null!;

    /// <summary>
    /// Navigation property to User (from Identity)
    /// </summary>
    public SecurityEntities.User User { get; set; } = null!;
}
