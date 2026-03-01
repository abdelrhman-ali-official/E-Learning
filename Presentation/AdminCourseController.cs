using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.CourseModels;
using System.Security.Claims;

namespace Presentation
{
    [Authorize(Roles = "Admin")]
    public class AdminCourseController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminCourseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("/api/admin/courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var instructorName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("DisplayName") ?? "Admin";
            var course = await _serviceManager.CourseService.CreateCourseAsync(dto, instructorId, instructorName);
            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, course);
        }

        [HttpPut("/api/admin/courses/{id:guid}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDTO dto)
        {
            var course = await _serviceManager.CourseService.UpdateCourseAsync(id, dto);
            return Ok(course);
        }

      
        [HttpDelete("/api/admin/courses/{id:guid}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await _serviceManager.CourseService.DeleteCourseAsync(id);
            return NoContent();
        }

       
        [HttpPatch("/api/admin/courses/{id:guid}/publish")]
        public async Task<IActionResult> PublishCourse(Guid id)
        {
            await _serviceManager.CourseService.PublishCourseAsync(id);
            return NoContent();
        }

        [HttpPatch("/api/admin/courses/{id:guid}/unpublish")]
        public async Task<IActionResult> UnpublishCourse(Guid id)
        {
            await _serviceManager.CourseService.UnpublishCourseAsync(id);
            return NoContent();
        }

      
        [HttpPatch("/api/admin/courses/{id:guid}/feature")]
        public async Task<IActionResult> ToggleFeature(Guid id)
        {
            await _serviceManager.CourseService.ToggleFeatureAsync(id);
            return NoContent();
        }

       
        [HttpGet("/api/admin/courses")]
        public async Task<IActionResult> GetAllCourses(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool includeUnpublished = true)
        {
            var courses = await _serviceManager.CourseService.GetAllCoursesAsync(
                pageIndex, pageSize, includeUnpublished);
            return Ok(courses);
        }

       
        [HttpGet("/api/admin/courses/{id:guid}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var course = await _serviceManager.CourseService.GetCourseByIdAsync(id);
            return Ok(course);
        }
    }
}
