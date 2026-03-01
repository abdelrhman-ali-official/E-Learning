using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.CourseModels;
using System.Security.Claims;

namespace Presentation
{
   
    [Authorize(Roles = "Instructor,Admin")]
    public class InstructorAnalyticsController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public InstructorAnalyticsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

       
        [HttpGet("/api/instructor/analytics")]
        [ProducesResponseType(typeof(InstructorAnalyticsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyAnalytics(CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.InstructorAnalyticsService
                .GetInstructorAnalyticsAsync(instructorId, cancellationToken);
            return Ok(result);
        }

       
        [HttpGet("/api/instructor/analytics/courses/{courseId:guid}")]
        [ProducesResponseType(typeof(CourseAnalyticsDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseAnalytics(
            Guid courseId,
            CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Re-use the full analytics and filter to the requested course.
            var allAnalytics = await _serviceManager.InstructorAnalyticsService
                .GetInstructorAnalyticsAsync(instructorId, cancellationToken);

            var detail = allAnalytics.CourseBreakdown
                .FirstOrDefault(c => c.CourseId == courseId);

            if (detail is null)
                return NotFound(new { message = "Course not found or does not belong to you." });

            return Ok(detail);
        }
    }
}
