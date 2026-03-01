using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.ContentModels;
using Shared.CourseModels;
using System.Security.Claims;

namespace Presentation
{
    [Authorize]
    public class CourseController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public CourseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

       
        [HttpGet("/api/courses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublishedCourses(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? category = null,
            [FromQuery] string? level = null)
        {
            var studentId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;
            var courses = await _serviceManager.CourseService.GetPublishedCoursesAsync(
                pageIndex, pageSize, category, level, studentId);
            return Ok(courses);
        }

        [HttpGet("/api/courses/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseBySlug(string slug)
        {
            var studentId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;
            var course = await _serviceManager.CourseService.GetCourseBySlugAsync(slug, studentId);
            return Ok(course);
        }

        [HttpGet("/api/courses/{courseId:guid}/access")]
        public async Task<IActionResult> CheckCourseAccess(Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var accessInfo = await _serviceManager.CourseService.CheckCourseAccessAsync(courseId, userId);
            return Ok(accessInfo);
        }

        [HttpPost("/api/courses/{courseId:guid}/enroll")]
        public async Task<IActionResult> EnrollInCourse(Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("DisplayName") ?? "Student";
            var enrollment = await _serviceManager.EnrollmentService.EnrollStudentAsync(
                courseId, userId, userName);
            return Ok(enrollment);
        }

        [HttpGet("/api/courses/{courseId:guid}/progress")]
        public async Task<IActionResult> GetCourseProgress(Guid courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var enrollment = await _serviceManager.EnrollmentService.GetEnrollmentAsync(courseId, userId);
            return Ok(enrollment);
        }

       
        [HttpPost("/api/courses/{courseId:guid}/review")]
        public async Task<IActionResult> SubmitReview(Guid courseId, [FromBody] CourseReviewDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("DisplayName") ?? "Anonymous";
            var review = await _serviceManager.CourseService.SubmitReviewAsync(courseId, userId, userName, dto);
            return Ok(review);
        }

       
        [HttpGet("/api/courses/{courseId:guid}/reviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseReviews(Guid courseId)
        {
            var reviews = await _serviceManager.CourseService.GetCourseReviewsAsync(courseId);
            return Ok(reviews);
        }

       
        [HttpGet("/api/student/enrollments")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var enrollments = await _serviceManager.EnrollmentService.GetStudentEnrollmentsAsync(userId);
            return Ok(enrollments);
        }

        [HttpGet("/api/courses/{courseId:guid}/contents")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<PublicCourseContentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseContentsPublic(Guid courseId)
        {
            var contents = await _serviceManager.ContentService.GetPublicCourseContentsAsync(courseId);
            return Ok(contents);
        }
    }
}
