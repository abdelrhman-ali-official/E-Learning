using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.SecurityModels;
using System.Security.Claims;

namespace Presentation
{
    /// <summary>
    /// Instructor: view all students registered in the platform.
    /// </summary>
    [Authorize(Roles = "Instructor,Admin")]
    public class InstructorStudentController : ApiController
    {
        private readonly IServiceManager _serviceManager;

        public InstructorStudentController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Get all students registered in the system.
        /// Each student record also includes how many of YOUR courses they are actively enrolled in.
        /// </summary>
        [HttpGet("/api/instructor/students")]
        [ProducesResponseType(typeof(IEnumerable<StudentSummaryDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStudents()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var students = await _serviceManager.AuthenticationService
                .GetAllStudentsAsync(instructorId);
            return Ok(students);
        }
    }
}
