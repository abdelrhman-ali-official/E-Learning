using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ContentModels;
using System.Security.Claims;

namespace Presentation
{
    [Authorize]
    public class PurchaseController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public PurchaseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

       
        [HttpGet("my-purchases")]
        public async Task<ActionResult<IEnumerable<PurchaseResultDTO>>> GetMyPurchases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found");

            var result = await _serviceManager.PurchaseService.GetUserPurchasesAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedResult<PurchaseResultDTO>>> GetAllPurchases(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _serviceManager.PurchaseService.GetAllPurchasesAsync(pageIndex, pageSize);
            return Ok(result);
        }
    }
}
