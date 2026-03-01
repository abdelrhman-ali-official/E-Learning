using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SubscriptionModels;
using System.Security.Claims;

namespace Presentation
{
    [Authorize]
    public class StudentSubscriptionController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public StudentSubscriptionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequestDTO dto, CancellationToken cancellationToken)
        {
            var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var subscription = await _serviceManager.SubscriptionService.SubscribeAsync(studentId, dto, cancellationToken);
            return Created(string.Empty, subscription);
        }

        [HttpGet("my-subscription")]
        public async Task<IActionResult> GetMySubscription(CancellationToken cancellationToken)
        {
            var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var subscription = await _serviceManager.SubscriptionService.GetStudentActiveSubscriptionAsync(studentId, cancellationToken);
            
            if (subscription == null)
                return NotFound(new { message = "No active subscription found" });
            
            return Ok(subscription);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSubscription(Guid id, CancellationToken cancellationToken)
        {
            var subscription = await _serviceManager.SubscriptionService.GetSubscriptionByIdAsync(id, cancellationToken);
            return Ok(subscription);
        }

        [HttpPost("calculate-price")]
        public async Task<IActionResult> CalculatePrice([FromBody] SubscribeRequestDTO dto, CancellationToken cancellationToken)
        {
            var finalPrice = await _serviceManager.SubscriptionService.CalculateFinalPriceAsync(
                dto.PackageId, 
                dto.BillingCycle, 
                dto.CouponCode, 
                cancellationToken);
            
            return Ok(new { finalPrice });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> CancelSubscription(Guid id, CancellationToken cancellationToken)
        {
            await _serviceManager.SubscriptionService.CancelSubscriptionAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
