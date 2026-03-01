using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.VideoModels;
using System.Security.Claims;

namespace Presentation;

[Authorize]
public class StudentVideoController : ApiController
{
    private readonly IServiceManager _serviceManager;

    public StudentVideoController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

   
    [HttpGet("/api/courses/{courseId:guid}/videos")]
    public async Task<IActionResult> GetCourseVideos(Guid courseId)
    {
        var result = await _serviceManager.VideoService.GetCourseVideosAsync(courseId);
        return Ok(result);
    }

  
    [HttpGet("/api/courses/{courseId:guid}/videos/{videoId:guid}")]
    public async Task<IActionResult> GetVideoStream(Guid courseId, Guid videoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var result = await _serviceManager.VideoService.GetVideoStreamAsync(courseId, videoId, userId, ipAddress);
        return Ok(result);
    }

  
    [HttpGet("/api/courses/{courseId:guid}/live")]
    public async Task<IActionResult> GetLiveStream(Guid courseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var result = await _serviceManager.LiveSessionService.GetLiveStreamAsync(courseId, userId, ipAddress);
        return Ok(result);
    }

   
    [HttpPost("/api/courses/{courseId:guid}/videos/{videoId:guid}/progress")]
    public async Task<IActionResult> UpdateVideoProgress(Guid courseId, Guid videoId, [FromBody] UpdateVideoProgressDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        var result = await _serviceManager.VideoService.UpdateVideoProgressAsync(videoId, userId, dto);
        return Ok(result);
    }

    [HttpGet("/api/courses/{courseId:guid}/videos/{videoId:guid}/progress")]
    public async Task<IActionResult> GetVideoProgress(Guid courseId, Guid videoId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        var result = await _serviceManager.VideoService.GetVideoProgressAsync(videoId, userId);
        
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("/api/courses/{courseId:guid}/progress")]
    public async Task<IActionResult> GetCourseProgress(Guid courseId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        var result = await _serviceManager.VideoService.GetCourseProgressAsync(courseId, userId);
        return Ok(result);
    }
}
