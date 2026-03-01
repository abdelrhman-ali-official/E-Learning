using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.VideoModels;

namespace Presentation;

[Authorize(Roles = "Admin")]
public class AdminVideoController : ApiController
{
    private readonly IServiceManager _serviceManager;

    public AdminVideoController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    
    [HttpPost("/api/admin/courses/{courseId:guid}/videos")]
    public async Task<IActionResult> CreateCourseVideo(Guid courseId, [FromBody] CreateCourseVideoDTO dto)
    {
        var result = await _serviceManager.VideoService.CreateCourseVideoAsync(courseId, dto);
        return CreatedAtAction(nameof(GetCourseVideos), new { courseId }, result);
    }

    
    [HttpPut("/api/admin/videos/{id:guid}")]
    public async Task<IActionResult> UpdateCourseVideo(Guid id, [FromBody] UpdateCourseVideoDTO dto)
    {
        var result = await _serviceManager.VideoService.UpdateCourseVideoAsync(id, dto);
        return Ok(result);
    }

    
    [HttpDelete("/api/admin/videos/{id:guid}")]
    public async Task<IActionResult> DeleteCourseVideo(Guid id)
    {
        await _serviceManager.VideoService.DeleteCourseVideoAsync(id);
        return NoContent();
    }


    [HttpGet("/api/admin/courses/{courseId:guid}/videos")]
    public async Task<IActionResult> GetCourseVideos(Guid courseId)
    {
        var result = await _serviceManager.VideoService.GetCourseVideosAsync(courseId);
        return Ok(result);
    }
}
