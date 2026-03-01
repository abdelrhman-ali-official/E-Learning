using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SubscriptionModels;

namespace Presentation
{
    [Authorize(Roles = "Admin")]
    public class AdminCouponController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminCouponController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDTO dto, CancellationToken cancellationToken)
        {
            var coupon = await _serviceManager.CouponService.CreateCouponAsync(dto, cancellationToken);
            return Created(string.Empty, coupon);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoupons(CancellationToken cancellationToken)
        {
            var coupons = await _serviceManager.CouponService.GetAllCouponsAsync(cancellationToken);
            return Ok(coupons);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetCouponByCode(string code, CancellationToken cancellationToken)
        {
            var coupon = await _serviceManager.CouponService.GetCouponByCodeAsync(code, cancellationToken);
            return Ok(coupon);
        }

        [HttpPut("{id:guid}/deactivate")]
        public async Task<IActionResult> DeactivateCoupon(Guid id, CancellationToken cancellationToken)
        {
            await _serviceManager.CouponService.DeactivateCouponAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
