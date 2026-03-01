using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.CourseModels;
using System.Security.Claims;

namespace Presentation
{
    /// <summary>
    /// Student: browse payment methods for a course and submit e-wallet payment proof.
    /// </summary>
    [Authorize]
    public class StudentCoursePaymentController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public StudentCoursePaymentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Get the available e-wallet payment methods for a course.
        /// The student should transfer money to one of these numbers then submit proof.
        /// </summary>
        [HttpGet("/api/courses/{courseId:guid}/payment-methods")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<EWalletMethodDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentMethods(Guid courseId)
        {
            var methods = await _serviceManager.CoursePaymentService.GetCourseEWalletMethodsAsync(courseId);
            return Ok(methods);
        }

        /// <summary>
        /// Submit a payment proof after transferring money out-of-system.
        /// After admin approval you will be automatically enrolled in the course.
        /// </summary>
        /// <remarks>
        /// Flow:
        /// 1. GET /api/courses/{courseId}/payment-methods  → pick a method, note the wallet number
        /// 2. Transfer money to that number via Vodafone Cash / InstaPay
        /// 3. POST here as multipart/form-data with the method ID, your wallet number, and screenshot image
        /// 4. Wait for admin approval → you'll be enrolled automatically
        /// </remarks>
        [HttpPost("/api/courses/{courseId:guid}/pay")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(CoursePaymentRequestDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SubmitPayment(
            Guid courseId,
            [FromForm] SubmitCoursePaymentDTO dto)
        {
            if (dto.PaymentProof == null || dto.PaymentProof.Length == 0)
                return BadRequest("PaymentProof image is required");

            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            string screenshotUrl;
            using (var stream = dto.PaymentProof.OpenReadStream())
            {
                screenshotUrl = await _serviceManager.StorageService.UploadFileAsync(
                    stream,
                    dto.PaymentProof.FileName,
                    dto.PaymentProof.ContentType);
            }

            var request = await _serviceManager.CoursePaymentService.SubmitPaymentRequestAsync(courseId, studentId, dto, screenshotUrl);
            return StatusCode(StatusCodes.Status201Created, request);
        }

        [HttpGet("/api/student/course-payments")]
        [ProducesResponseType(typeof(IEnumerable<CoursePaymentRequestDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyPayments()
        {
            var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var requests = await _serviceManager.CoursePaymentService.GetMyPaymentRequestsAsync(studentId);
            return Ok(requests);
        }
    }
}
