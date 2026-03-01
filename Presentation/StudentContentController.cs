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
        var result = await _serviceManager.ContentService.GetCourseContentsAsync(courseId);
        return Ok(result.Where(c => c.IsVisible));
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
}
