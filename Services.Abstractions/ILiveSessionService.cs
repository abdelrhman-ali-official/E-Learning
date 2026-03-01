using Shared.VideoModels;

namespace Services.Abstractions;

/// <summary>
/// Service for managing live streaming sessions (Zoom, YouTube, etc.)
/// </summary>
public interface ILiveSessionService
{
    /// <summary>
    /// Creates a new live session for a course (Admin or Instructor)
    /// </summary>
    Task<LiveSessionResponseDTO> CreateLiveSessionAsync(Guid courseId, CreateLiveSessionDTO dto, string instructorId);

    /// <summary>
    /// Updates an existing live session (Admin or owner Instructor)
    /// </summary>
    Task<LiveSessionResponseDTO> UpdateLiveSessionAsync(Guid sessionId, UpdateLiveSessionDTO dto);

    /// <summary>
    /// Deletes a live session (Admin or owner Instructor)
    /// </summary>
    Task DeleteLiveSessionAsync(Guid sessionId, string userId, bool isAdmin);

    /// <summary>
    /// Activates a live session (Admin only)
    /// </summary>
    Task ActivateLiveSessionAsync(Guid sessionId);

    /// <summary>
    /// Deactivates a live session (Admin only)
    /// </summary>
    Task DeactivateLiveSessionAsync(Guid sessionId);

    /// <summary>
    /// Attaches recording video ID to a live session (Admin only)
    /// </summary>
    Task AttachRecordingAsync(Guid sessionId, AttachRecordingDTO dto);

    /// <summary>
    /// Deactivates a live session - only allowed by the owning instructor
    /// </summary>
    Task DeactivateLiveSessionByInstructorAsync(Guid sessionId, string instructorId);

    /// <summary>
    /// Attaches a recording link to a live session - only allowed by the owning instructor
    /// </summary>
    Task AttachRecordingByInstructorAsync(Guid sessionId, string instructorId, AttachRecordingDTO dto);

    /// <summary>
    /// Gets active or upcoming live sessions for a course (Admin/Instructor)
    /// </summary>
    Task<IEnumerable<LiveSessionResponseDTO>> GetCourseLiveSessionsAsync(Guid courseId);

    /// <summary>
    /// Gets upcoming live sessions for courses the student is enrolled in
    /// </summary>
    Task<IEnumerable<LiveSessionResponseDTO>> GetUpcomingSessionsForStudentAsync(string userId);

    /// <summary>
    /// Gets a single live session by ID (with access validation for students)
    /// </summary>
    Task<LiveSessionResponseDTO> GetLiveSessionByIdAsync(Guid sessionId, string userId, bool isEnrolled);

    /// <summary>
    /// Gets live stream with access validation
    /// Validates:
    /// - Course purchase
    /// - Time window (ScheduledStart <= Now <= ScheduledEnd)
    /// - IsActive = true
    /// Returns recording if live ended and available
    /// </summary>
    Task<LiveStreamResponseDTO> GetLiveStreamAsync(Guid courseId, string userId, string ipAddress);

    /// <summary>
    /// Background job: Deactivates expired live sessions
    /// </summary>
    Task DeactivateExpiredSessionsAsync();

    /// <summary>
    /// Generates a short-lived RS256 JaaS JWT for the calling user to join the session.
    /// Validates: session active state, time window, and course enrollment (students).
    /// </summary>
    Task<JoinLiveSessionResponseDTO> JoinLiveSessionAsync(
        Guid sessionId,
        string userId,
        string userName,
        string email,
        bool isModerator);
}
