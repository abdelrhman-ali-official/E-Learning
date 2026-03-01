using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.ContentModels;
using System.Security.Claims;

namespace Presentation;

[Authorize]
public class StudentContentController : ApiController
{
    private readonly IServiceManager _serviceManager;

    public StudentContentController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpGet("/api/student/courses/{courseId:guid}/content")]
    [ProducesResponseType(typeof(IEnumerable<ContentResultDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCourseContent(Guid courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.CourseAccessService.ValidateCourseAccessAsync(courseId, userId);
        var result = await _serviceManager.ContentService.GetStudentCourseContentsAsync(courseId, userId);
        return Ok(result);
    }

    [HttpGet("/api/student/courses/{courseId:guid}/content/{id:int}")]
    [ProducesResponseType(typeof(ContentResultDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetContentById(Guid courseId, int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.CourseAccessService.ValidateCourseAccessAsync(courseId, userId);
        var result = await _serviceManager.ContentService.GetCourseContentByIdAsync(courseId, id);
        if (!result.IsVisible)
            return Forbid();
        return Ok(result);
    }

    // ── Content Completion Tracking ─────────────────────────────────────

    /// <summary>Mark a content item as complete or incomplete.</summary>
    [HttpPut("/api/student/courses/{courseId:guid}/content/{id:int}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkContentComplete(Guid courseId, int id, [FromBody] MarkContentCompleteDTO body)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.CourseAccessService.ValidateCourseAccessAsync(courseId, userId);
        await _serviceManager.ContentService.MarkContentCompleteAsync(courseId, id, userId, body.IsComplete);
        return NoContent();
    }

    /// <summary>Get per-item completion status and overall progress for a course.</summary>
    [HttpGet("/api/student/courses/{courseId:guid}/content/progress")]
    [ProducesResponseType(typeof(ContentProgressDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCourseProgress(Guid courseId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.CourseAccessService.ValidateCourseAccessAsync(courseId, userId);
        var result = await _serviceManager.ContentService.GetCourseContentProgressAsync(courseId, userId);
        return Ok(result);
    }
}
