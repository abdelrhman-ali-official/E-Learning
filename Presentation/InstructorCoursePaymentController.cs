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
    public class InstructorCoursePaymentController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public InstructorCoursePaymentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

      
        [HttpPost("/api/instructor/courses/{courseId:guid}/payment-methods")]
        [ProducesResponseType(typeof(EWalletMethodDTO), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddPaymentMethod(Guid courseId, [FromBody] AddEWalletMethodDTO dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var method = await _serviceManager.CoursePaymentService.AddEWalletMethodAsync(courseId, instructorId, dto);
            return CreatedAtAction(nameof(GetPaymentMethods), new { courseId }, method);
        }

        [HttpPut("/api/instructor/courses/{courseId:guid}/payment-methods/{methodId:int}")]
        [ProducesResponseType(typeof(EWalletMethodDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePaymentMethod(Guid courseId, int methodId, [FromBody] AddEWalletMethodDTO dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var method = await _serviceManager.CoursePaymentService.UpdateEWalletMethodAsync(methodId, instructorId, dto);
            return Ok(method);
        }

       
        [HttpDelete("/api/instructor/courses/{courseId:guid}/payment-methods/{methodId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemovePaymentMethod(Guid courseId, int methodId)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _serviceManager.CoursePaymentService.RemoveEWalletMethodAsync(methodId, instructorId);
            return NoContent();
        }

        [HttpGet("/api/instructor/courses/{courseId:guid}/payment-methods")]
        [ProducesResponseType(typeof(IEnumerable<EWalletMethodDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentMethods(Guid courseId)
        {
            var methods = await _serviceManager.CoursePaymentService.GetCourseEWalletMethodsAsync(courseId);
            return Ok(methods);
        }

       
        [HttpGet("/api/instructor/courses/{courseId:guid}/payments")]
        [ProducesResponseType(typeof(PaginatedResult<CoursePaymentRequestDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentRequests(
            Guid courseId,
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.CoursePaymentService
                .GetCoursePaymentRequestsAsync(courseId, instructorId, status, pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("/api/instructor/courses/{courseId:guid}/payments/{requestId:int}")]
        [ProducesResponseType(typeof(CoursePaymentRequestDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentRequest(Guid courseId, int requestId)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var dto = await _serviceManager.CoursePaymentService
                .GetCoursePaymentRequestByIdAsync(requestId, instructorId);
            return Ok(dto);
        }

        
        [HttpPut("/api/instructor/courses/{courseId:guid}/payments/{requestId:int}/review")]
        [ProducesResponseType(typeof(CoursePaymentRequestDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReviewPaymentRequest(
            Guid courseId, int requestId, [FromBody] ReviewCoursePaymentDTO dto)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _serviceManager.CoursePaymentService
                .InstructorReviewPaymentRequestAsync(requestId, instructorId, dto);
            return Ok(result);
        }
    }
}
