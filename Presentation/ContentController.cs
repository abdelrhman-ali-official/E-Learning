using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared;
using Shared.ContentModels;

namespace Presentation
{
    [Authorize(Roles = "Admin")]
    public class ContentController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public ContentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

       
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<ContentResultDTO>>> GetAllContents(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _serviceManager.ContentService.GetAllContentsAsync(pageIndex, pageSize);
            return Ok(result);
        }

       
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteContent(int id)
        {
            await _serviceManager.ContentService.DeleteContentAsync(id);
            return NoContent();
        }

      
        [HttpPatch("{id:int}/visibility")]
        public async Task<ActionResult<ContentResultDTO>> ToggleVisibility(int id)
        {
            var result = await _serviceManager.ContentService.ToggleVisibilityAsync(id);
            return Ok(result);
        }
    }
}
