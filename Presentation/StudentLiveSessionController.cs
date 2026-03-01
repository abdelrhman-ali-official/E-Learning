using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.VideoModels;
using System.Security.Claims;

namespace Presentation;

/// <summary>
/// Student: view and access live sessions for enrolled courses.
/// </summary>
[Authorize]
public class StudentLiveSessionController : ApiController
{
    private readonly IServiceManager _serviceManager;

    public StudentLiveSessionController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    /// <summary>
    /// Get all upcoming live sessions across every course the student is enrolled in.
    /// </summary>
    [HttpGet("/api/student/live-sessions/upcoming")]
    [ProducesResponseType(typeof(IEnumerable<LiveSessionResponseDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingSessions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var sessions = await _serviceManager.LiveSessionService.GetUpcomingSessionsForStudentAsync(userId);
        return Ok(sessions);
    }

    /// <summary>
    /// Get all live sessions (upcoming, live now, and past) for a specific course.
    /// Requires the student to be enrolled in that course.
    /// </summary>
    /// <param name="courseId">The course ID.</param>
    [HttpGet("/api/student/courses/{courseId:guid}/live-sessions")]
    [ProducesResponseType(typeof(IEnumerable<LiveSessionResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCourseLiveSessions(Guid courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Throws CourseAccessDeniedException (403) if not enrolled
        await _serviceManager.CourseAccessService.ValidateCourseAccessAsync(courseId, userId);

        var sessions = await _serviceManager.LiveSessionService.GetCourseLiveSessionsAsync(courseId);
        return Ok(sessions);
    }

    /// <summary>
    /// Get details of a single live session by ID.
    /// Requires the student to be enrolled in the course that owns this session.
    /// </summary>
    /// <param name="sessionId">The live session ID.</param>
    [HttpGet("/api/student/live-sessions/{sessionId:guid}")]
    [ProducesResponseType(typeof(LiveSessionResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLiveSessionById(Guid sessionId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        // isEnrolled = false → service will call ValidateCourseAccessAsync internally
        var session = await _serviceManager.LiveSessionService.GetLiveSessionByIdAsync(sessionId, userId, false);
        return Ok(session);
    }

   
    [HttpPost("/api/student/live-sessions/{sessionId:guid}/join")]
    [ProducesResponseType(typeof(JoinLiveSessionResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JoinLiveSession(Guid sessionId)
    {
        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? userId;
        var email    = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        var result = await _serviceManager.LiveSessionService
            .JoinLiveSessionAsync(sessionId, userId, userName, email, isModerator: false);
        return Ok(result);
    }
}
