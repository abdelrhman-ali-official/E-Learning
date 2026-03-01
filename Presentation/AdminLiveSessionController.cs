using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.VideoModels;
using System.Security.Claims;

namespace Presentation;

[Authorize(Roles = "Admin")]
public class AdminLiveSessionController : ApiController
{
    private readonly IServiceManager _serviceManager;

    public AdminLiveSessionController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

   
    [HttpPost("/api/admin/courses/{courseId:guid}/live")]
    [ProducesResponseType(typeof(LiveSessionResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateLiveSession(Guid courseId, [FromBody] CreateLiveSessionDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _serviceManager.LiveSessionService.CreateLiveSessionAsync(courseId, dto, userId);
        return CreatedAtAction(nameof(GetCourseLiveSessions), new { courseId }, result);
    }

   
    [HttpPut("/api/admin/live/{id:guid}")]
    [ProducesResponseType(typeof(LiveSessionResponseDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateLiveSession(Guid id, [FromBody] UpdateLiveSessionDTO dto)
    {
        var result = await _serviceManager.LiveSessionService.UpdateLiveSessionAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("/api/admin/live/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteLiveSession(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.LiveSessionService.DeleteLiveSessionAsync(id, userId, isAdmin: true);
        return NoContent();
    }

  
    [HttpPatch("/api/admin/live/{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateLiveSession(Guid id)
    {
        await _serviceManager.LiveSessionService.ActivateLiveSessionAsync(id);
        return NoContent();
    }

   
    [HttpPatch("/api/admin/live/{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeactivateLiveSession(Guid id)
    {
        await _serviceManager.LiveSessionService.DeactivateLiveSessionAsync(id);
        return NoContent();
    }

    [HttpPatch("/api/admin/live/{id:guid}/attach-recording")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AttachRecording(Guid id, [FromBody] AttachRecordingDTO dto)
    {
        await _serviceManager.LiveSessionService.AttachRecordingAsync(id, dto);
        return NoContent();
    }

  
    [HttpGet("/api/admin/courses/{courseId:guid}/live")]
    [ProducesResponseType(typeof(IEnumerable<LiveSessionResponseDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCourseLiveSessions(Guid courseId)
    {
        var result = await _serviceManager.LiveSessionService.GetCourseLiveSessionsAsync(courseId);
        return Ok(result);
    }

    
    [HttpPost("/api/admin/live/{id:guid}/join")]
    [ProducesResponseType(typeof(JoinLiveSessionResponseDTO), StatusCodes.Status200OK)]
    public async Task<IActionResult> JoinLiveSession(Guid id)
    {
        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? userId;
        var email    = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        var result = await _serviceManager.LiveSessionService
            .JoinLiveSessionAsync(id, userId, userName, email, isModerator: true);
        return Ok(result);
    }
}
