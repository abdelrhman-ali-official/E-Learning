using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SecurityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation
{
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public AdminUserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

       
        [AllowAnonymous]
        [HttpGet("test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "AdminUserController is working!", timestamp = System.DateTime.UtcNow });
        }

        

        [HttpPost("instructors")]
        [ProducesResponseType(typeof(UserResultDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateInstructor([FromBody] CreateInstructorDTO dto)
        {
            var result = await _serviceManager.AuthenticationService.CreateInstructorAsync(dto);
            return CreatedAtAction(nameof(GetAllInstructors), new { }, result);
        }

        
        [HttpGet("instructors")]
        [ProducesResponseType(typeof(List<UserResultDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInstructors()
        {
            var instructors = await _serviceManager.AuthenticationService.GetAllInstructorsAsync();
            return Ok(instructors);
        }

        
        [HttpPost("users/{email}/fix-admin-role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> FixAdminRole(string email)
        {
            var result = await _serviceManager.AuthenticationService.FixAdminRoles(email);
            return Ok(new { success = result, message = "Admin role fixed successfully" });
        }
    }
}
