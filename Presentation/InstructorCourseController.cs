using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.CourseModels;
using System.Security.Claims;

namespace Presentation
{
    [Authorize(Roles = "Instructor,Admin")]
    public class InstructorCourseController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public InstructorCourseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }


        [HttpGet("/api/instructor/courses")]
        [ProducesResponseType(typeof(PaginatedResult<CourseResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCourses(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var courses = await _serviceManager.CourseService.GetInstructorCoursesAsync(
                instructorId, pageIndex, pageSize);
            return Ok(courses);
        }

        [HttpGet("/api/instructor/courses/{id:guid}")]
        [ProducesResponseType(typeof(CourseDetailDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCourse(Guid id)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var course = await _serviceManager.CourseService.GetCourseByIdAsync(id);
            if (course.InstructorId != instructorId && !User.IsInRole("Admin"))
                return Forbid();
            return Ok(course);
        }

        [HttpPost("/api/instructor/courses")]
        [ProducesResponseType(typeof(CourseResponseDTO), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var instructorName = User.FindFirstValue(ClaimTypes.Name)
                ?? User.FindFirstValue("DisplayName") ?? "Instructor";
            var course = await _serviceManager.CourseService.CreateCourseAsync(
                dto, instructorId, instructorName);
            return CreatedAtAction(nameof(GetMyCourse), new { id = course.Id }, course);
        }

        [HttpPut("/api/instructor/courses/{id:guid}")]
        [ProducesResponseType(typeof(CourseResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMyCourse(Guid id, [FromBody] UpdateCourseDTO dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var existing = await _serviceManager.CourseService.GetCourseByIdAsync(id);
            if (existing.InstructorId != instructorId && !User.IsInRole("Admin"))
                return Forbid();
            var course = await _serviceManager.CourseService.UpdateCourseAsync(id, dto);
            return Ok(course);
        }

        
        [HttpPatch("/api/instructor/courses/{id:guid}/pricing")]
        [ProducesResponseType(typeof(CourseResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePricing(Guid id, [FromBody] UpdateCoursePricingDTO dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var course = await _serviceManager.CourseService.UpdateCoursePricingAsync(
                id, instructorId, dto);
            return Ok(course);
        }

        [HttpDelete("/api/instructor/courses/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMyCourse(Guid id)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var existing = await _serviceManager.CourseService.GetCourseByIdAsync(id);
            if (existing.InstructorId != instructorId && !User.IsInRole("Admin"))
                return Forbid();
            await _serviceManager.CourseService.DeleteCourseAsync(id);
            return NoContent();
        }

       
        [HttpPatch("/api/instructor/courses/{id:guid}/publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PublishMyCourse(Guid id)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var existing = await _serviceManager.CourseService.GetCourseByIdAsync(id);
            if (existing.InstructorId != instructorId && !User.IsInRole("Admin"))
                return Forbid();
            await _serviceManager.CourseService.PublishCourseAsync(id);
            return Ok(new { message = "Course published successfully" });
        }

        [HttpPatch("/api/instructor/courses/{id:guid}/unpublish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UnpublishMyCourse(Guid id)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var existing = await _serviceManager.CourseService.GetCourseByIdAsync(id);
            if (existing.InstructorId != instructorId && !User.IsInRole("Admin"))
                return Forbid();
            await _serviceManager.CourseService.UnpublishCourseAsync(id);
            return Ok(new { message = "Course unpublished successfully" });
        }
    }
}

