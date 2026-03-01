using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Presentation
{
    [Authorize(Roles = "Admin")]
    public class AdminSubscriptionController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminSubscriptionController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubscriptions(CancellationToken cancellationToken)
        {
            var subscriptions = await _serviceManager.SubscriptionService.GetAllSubscriptionsAsync(cancellationToken);
            return Ok(subscriptions);
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics(CancellationToken cancellationToken)
        {
            var analytics = await _serviceManager.SubscriptionAnalyticsService.GetAnalyticsAsync(cancellationToken);
            return Ok(analytics);
        }
    }
}
