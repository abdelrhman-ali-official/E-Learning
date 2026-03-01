using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ContentModels;
using Shared.CourseModels;
using Shared.SubscriptionModels;
using System.Security.Claims;

namespace Presentation
{
   
    [Authorize(Roles = "Admin")]
    public class AdminPaymentController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminPaymentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

      
        [HttpGet("/api/admin/payments/subscription/pending")]
        public async Task<IActionResult> GetPendingSubscriptionPayments(CancellationToken cancellationToken)
        {
            var payments = await _serviceManager.PaymentRequestService.GetPendingPaymentsAsync(cancellationToken);
            return Ok(payments);
        }

        [HttpPatch("/api/admin/payments/subscription/{id:guid}/approve")]
        public async Task<IActionResult> ApproveSubscriptionPayment(
            Guid id, 
            [FromBody] ApprovePaymentDTO dto, 
            CancellationToken cancellationToken)
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var payment = await _serviceManager.PaymentRequestService.ApprovePaymentAsync(
                id, adminId, dto, cancellationToken);
            return Ok(payment);
        }

        [HttpPatch("/api/admin/payments/subscription/{id:guid}/reject")]
        public async Task<IActionResult> RejectSubscriptionPayment(
            Guid id, 
            [FromBody] RejectPaymentDTO dto, 
            CancellationToken cancellationToken)
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var payment = await _serviceManager.PaymentRequestService.RejectPaymentAsync(
                id, adminId, dto, cancellationToken);
            return Ok(payment);
        }

        [HttpGet("/api/admin/payments/manual/pending")]
        public async Task<ActionResult<PaginatedResult<ManualPaymentRequestResultDTO>>> GetPendingManualPayments(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _serviceManager.ManualPaymentService.GetPendingRequestsAsync(pageIndex, pageSize);
            return Ok(result);
        }

        
        [HttpPatch("/api/admin/payments/manual/{id:int}/approve")]
        public async Task<ActionResult<ManualPaymentRequestResultDTO>> ApproveManualPayment(int id)
        {
            var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found");

            var reviewDto = new ReviewManualPaymentDTO { Status = 2 }; // Approved
            var result = await _serviceManager.ManualPaymentService.ReviewManualPaymentAsync(
                id, reviewDto, reviewerId);
            return Ok(result);
        }

       
        [HttpPatch("/api/admin/payments/manual/{id:int}/reject")]
        public async Task<ActionResult<ManualPaymentRequestResultDTO>> RejectManualPayment(
            int id,
            [FromQuery] string? reason = null)
        {
            var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found");

            var dto = new ReviewManualPaymentDTO 
            { 
                Status = 3, // Rejected
                RejectionReason = reason 
            };

            var result = await _serviceManager.ManualPaymentService.ReviewManualPaymentAsync(
                id, dto, reviewerId);
            return Ok(result);
        }

       
        [HttpGet("/api/admin/payments/course/pending")]
        public async Task<ActionResult<PaginatedResult<CoursePaymentRequestDTO>>> GetPendingCoursePayments(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _serviceManager.CoursePaymentService.GetPendingPaymentRequestsAsync(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet("/api/admin/payments/course/all")]
        public async Task<ActionResult<PaginatedResult<CoursePaymentRequestDTO>>> GetAllCoursePayments(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _serviceManager.CoursePaymentService.GetAllPaymentRequestsAsync(pageIndex, pageSize);
            return Ok(result);
        }

       
        [HttpPatch("/api/admin/payments/course/{id:int}/approve")]
        public async Task<ActionResult<CoursePaymentRequestDTO>> ApproveCoursePayment(int id)
        {
            var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found");

            var result = await _serviceManager.CoursePaymentService.ReviewPaymentRequestAsync(
                id, reviewerId, new ReviewCoursePaymentDTO { Approve = true });
            return Ok(result);
        }

       
        [HttpPatch("/api/admin/payments/course/{id:int}/reject")]
        public async Task<ActionResult<CoursePaymentRequestDTO>> RejectCoursePayment(
            int id,
            [FromBody] ReviewCoursePaymentDTO dto)
        {
            var reviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found");

            var rejectDto = new ReviewCoursePaymentDTO { Approve = false, RejectionReason = dto.RejectionReason };
            var result = await _serviceManager.CoursePaymentService.ReviewPaymentRequestAsync(id, reviewerId, rejectDto);
            return Ok(result);
        }
    }
}

