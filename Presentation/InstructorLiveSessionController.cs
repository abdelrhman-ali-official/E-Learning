using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.VideoModels;
using System.Security.Claims;

namespace Presentation;


[Authorize] // Authenticated users (Instructors)
public class InstructorLiveSessionController : ApiController
{
    private readonly IServiceManager _serviceManager;

    public InstructorLiveSessionController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost("/api/instructor/courses/{courseId:guid}/live-sessions")]
    public async Task<IActionResult> CreateLiveSession(Guid courseId, [FromBody] CreateLiveSessionDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await _serviceManager.LiveSessionService.CreateLiveSessionAsync(courseId, dto, userId);
        return CreatedAtAction(nameof(GetLiveSessionById), new { id = result.Id }, result);
    }

    
    [HttpPut("/api/instructor/live-sessions/{id:guid}")]
    public async Task<IActionResult> UpdateLiveSession(Guid id, [FromBody] UpdateLiveSessionDTO dto)
    {
        var result = await _serviceManager.LiveSessionService.UpdateLiveSessionAsync(id, dto);
        return Ok(result);
    }

   
    [HttpDelete("/api/instructor/live-sessions/{id:guid}")]
    public async Task<IActionResult> DeleteLiveSession(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        await _serviceManager.LiveSessionService.DeleteLiveSessionAsync(id, userId, isAdmin);
        return NoContent();
    }

  
    [HttpGet("/api/instructor/courses/{courseId:guid}/live-sessions")]
    public async Task<IActionResult> GetCourseLiveSessions(Guid courseId)
    {
        var result = await _serviceManager.LiveSessionService.GetCourseLiveSessionsAsync(courseId);
        return Ok(result);
    }

   
    [HttpGet("/api/instructor/live-sessions/{id:guid}")]
    public async Task<IActionResult> GetLiveSessionById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _serviceManager.LiveSessionService.GetLiveSessionByIdAsync(id, userId, true);
        return Ok(result);
    }

   
    [HttpPatch("/api/instructor/live-sessions/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateLiveSession(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.LiveSessionService.DeactivateLiveSessionByInstructorAsync(id, userId);
        return NoContent();
    }

   
    [HttpPatch("/api/instructor/live-sessions/{id:guid}/attach-recording")]
    public async Task<IActionResult> AttachRecording(Guid id, [FromBody] AttachRecordingDTO dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _serviceManager.LiveSessionService.AttachRecordingByInstructorAsync(id, userId, dto);
        return NoContent();
    }

 
    [HttpPost("/api/instructor/live-sessions/{id:guid}/join")]
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
