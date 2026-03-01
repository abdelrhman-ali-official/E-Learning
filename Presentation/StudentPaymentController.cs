using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ContentModels;
using Shared.SubscriptionModels;
using System.Security.Claims;

namespace Presentation
{
    [Authorize]
    public class StudentPaymentController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public StudentPaymentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

      
        [HttpPost("/api/payments/subscription/submit")]
        public async Task<IActionResult> SubmitSubscriptionPayment(
            [FromForm] SubmitPaymentRequestDTO dto, 
            CancellationToken cancellationToken)
        {
            var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var payment = await _serviceManager.PaymentRequestService.SubmitPaymentRequestAsync(
                studentId, dto, cancellationToken);
            return Created(string.Empty, payment);
        }

        [HttpGet("/api/payments/subscription/{subscriptionId:guid}")]
        public async Task<IActionResult> GetPaymentsBySubscription(
            Guid subscriptionId, 
            CancellationToken cancellationToken)
        {
            var payments = await _serviceManager.PaymentRequestService.GetPaymentsBySubscriptionAsync(
                subscriptionId, cancellationToken);
            return Ok(payments);
        }

        [HttpPost("/api/payments/manual")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ManualPaymentRequestResultDTO>> CreateManualPaymentRequest(
            [FromForm] CreateManualPaymentRequestDTO dto,
            [FromForm] IFormFile screenshot)
        {
            if (screenshot == null || screenshot.Length == 0)
                return BadRequest("Screenshot image is required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found");

            // Upload screenshot to Cloudflare R2
            string screenshotUrl;
            using (var stream = screenshot.OpenReadStream())
            {
                screenshotUrl = await _serviceManager.StorageService.UploadFileAsync(
                    stream,
                    screenshot.FileName,
                    screenshot.ContentType);
            }

            var result = await _serviceManager.ManualPaymentService.CreateManualPaymentRequestAsync(
                dto, 
                userId, 
                screenshotUrl);
            
            return CreatedAtAction(nameof(GetMyManualPaymentRequests), new { }, result);
        }

        
        [HttpGet("/api/payments/manual/my-requests")]
        public async Task<ActionResult<IEnumerable<ManualPaymentRequestResultDTO>>> GetMyManualPaymentRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found");

            var result = await _serviceManager.ManualPaymentService.GetUserRequestsAsync(userId);
            return Ok(result);
        }
    }
}

