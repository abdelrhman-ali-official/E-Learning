using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.ContentModels;
using System.Security.Claims;

namespace Presentation;

[Authorize(Roles = "Instructor,Admin")]
public class InstructorContentController : ApiController
{
    private readonly IServiceManager _serviceManager;

    public InstructorContentController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost("/api/instructor/courses/{courseId:guid}/content")]
    [ProducesResponseType(typeof(ContentResultDTO), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateContent(Guid courseId, [FromBody] CreateContentDTO dto)
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _serviceManager.ContentService.CreateCourseContentAsync(courseId, instructorId, dto);
        return CreatedAtAction(nameof(GetContentById), new { courseId, id = result.Id }, result);
    }

    [HttpPut("/api/instructor/courses/{courseId:guid}/content/{id:int}")]
    [ProducesResponseType(typeof(ContentResultDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateContent(Guid courseId, int id, [FromBody] UpdateContentDTO dto)
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _serviceManager.ContentService.UpdateCourseContentAsync(id, instructorId, dto);
        return Ok(result);
    }

    [HttpDelete("/api/instructor/courses/{courseId:guid}/content/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteContent(Guid courseId, int id)
    {
        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.ContentService.DeleteCourseContentAsync(id, instructorId);
        return NoContent();
    }

    [HttpGet("/api/instructor/courses/{courseId:guid}/content")]
    [ProducesResponseType(typeof(IEnumerable<ContentResultDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseContents(Guid courseId)
    {
        var result = await _serviceManager.ContentService.GetCourseContentsAsync(courseId);
        return Ok(result);
    }

    [HttpGet("/api/instructor/courses/{courseId:guid}/content/{id:int}")]
    [ProducesResponseType(typeof(ContentResultDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContentById(Guid courseId, int id)
    {
        var result = await _serviceManager.ContentService.GetCourseContentByIdAsync(courseId, id);
        return Ok(result);
    }
}
