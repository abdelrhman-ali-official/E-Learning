using Shared.VideoModels;

namespace Services.Abstractions;

/// <summary>
/// Service for managing course videos and student access
/// </summary>
public interface IVideoService
{
    /// <summary>
    /// Creates a new course video (Admin only)
    /// </summary>
    Task<CourseVideoResponseDTO> CreateCourseVideoAsync(Guid courseId, CreateCourseVideoDTO dto);

    /// <summary>
    /// Updates an existing course video (Admin only)
    /// </summary>
    Task<CourseVideoResponseDTO> UpdateCourseVideoAsync(Guid videoId, UpdateCourseVideoDTO dto);

    /// <summary>
    /// Deletes a course video (Admin only)
    /// </summary>
    Task DeleteCourseVideoAsync(Guid videoId);

    /// <summary>
    /// Gets all videos for a course (ordered by OrderIndex)
    /// Returns limited info for students, full info for admins
    /// </summary>
    Task<IEnumerable<CourseVideoResponseDTO>> GetCourseVideosAsync(Guid courseId);

    /// <summary>
    /// Gets video stream URL with access validation
    /// Validates purchase for non-preview videos
    /// Logs access attempt
    /// </summary>
    Task<VideoStreamResponseDTO> GetVideoStreamAsync(Guid courseId, Guid videoId, string userId, string ipAddress);

    /// <summary>
    /// Updates student video progress
    /// Auto-completes if WatchedSeconds >= 90% of duration
    /// </summary>
    Task<VideoProgressResponseDTO> UpdateVideoProgressAsync(Guid videoId, string userId, UpdateVideoProgressDTO dto);

    /// <summary>
    /// Gets student progress for a specific video
    /// </summary>
    Task<VideoProgressResponseDTO?> GetVideoProgressAsync(Guid videoId, string userId);

    /// <summary>
    /// Gets all video progress for a student in a course
    /// </summary>
    Task<IEnumerable<VideoProgressResponseDTO>> GetCourseProgressAsync(Guid courseId, string userId);
}
