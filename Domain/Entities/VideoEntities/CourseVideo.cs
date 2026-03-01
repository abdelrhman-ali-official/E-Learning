namespace Domain.Entities.VideoEntities;

/// <summary>
/// Represents a YouTube unlisted video associated with a course
/// </summary>
public class CourseVideo : BaseEntity<Guid>
{
    /// <summary>
    /// Video title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Video description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ONLY the YouTube Video ID (not full URL)
    /// Example: "dQw4w9WgXcQ" from https://www.youtube.com/watch?v=dQw4w9WgXcQ
    /// </summary>
    public string VideoId { get; set; } = string.Empty;

    /// <summary>
    /// Duration in seconds
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// Foreign key to Course
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Order index for playlist ordering
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// If true, accessible to everyone (preview/demo)
    /// If false, requires course purchase validation
    /// </summary>
    public bool IsPreview { get; set; }

    /// <summary>
    /// Timestamp when created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property to Course
    /// </summary>
    public CourseEntities.Course Course { get; set; } = null!;

    /// <summary>
    /// Navigation property to video progress records
    /// </summary>
    public ICollection<VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress>();

    /// <summary>
    /// Navigation property to access logs
    /// </summary>
    public ICollection<VideoAccessLog> AccessLogs { get; set; } = new List<VideoAccessLog>();
}
