namespace Domain.Entities.VideoEntities;

/// <summary>
/// Logs every video access attempt for security auditing and analytics
/// </summary>
public class VideoAccessLog : BaseEntity<Guid>
{
    /// <summary>
    /// Foreign key to User (IdentityUser)
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Foreign key to Course
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Video ID that was accessed (CourseVideo or LiveSession)
    /// </summary>
    public Guid VideoId { get; set; }

    /// <summary>
    /// Type of video accessed (CourseVideo or LiveSession)
    /// </summary>
    public string VideoType { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when access occurred
    /// </summary>
    public DateTime AccessedAt { get; set; }

    /// <summary>
    /// IP Address of the requester
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to Course
    /// </summary>
    public CourseEntities.Course Course { get; set; } = null!;

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public SecurityEntities.User User { get; set; } = null!;
}
