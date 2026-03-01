using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SubscriptionModels;

namespace Presentation
{
    [Authorize(Roles = "Admin")]
    public class AdminPackageController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminPackageController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] CreatePackageDTO dto, CancellationToken cancellationToken)
        {
            var package = await _serviceManager.PackageService.CreatePackageAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetPackage), new { id = package.Id }, package);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePackage(Guid id, [FromBody] UpdatePackageDTO dto, CancellationToken cancellationToken)
        {
            var package = await _serviceManager.PackageService.UpdatePackageAsync(id, dto, cancellationToken);
            return Ok(package);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePackage(Guid id, CancellationToken cancellationToken)
        {
            await _serviceManager.PackageService.DeletePackageAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPackage(Guid id, CancellationToken cancellationToken)
        {
            var package = await _serviceManager.PackageService.GetPackageByIdAsync(id, cancellationToken);
            return Ok(package);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPackages([FromQuery] bool activeOnly = false, CancellationToken cancellationToken = default)
        {
            var packages = await _serviceManager.PackageService.GetAllPackagesAsync(activeOnly, cancellationToken);
            return Ok(packages);
        }
    }
}
