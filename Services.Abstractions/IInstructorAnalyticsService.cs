using Shared.CourseModels;

namespace Services.Abstractions
{
    public interface IInstructorAnalyticsService
    {
        /// <summary>
        /// Returns a comprehensive analytics snapshot for the given instructor.
        /// </summary>
        Task<InstructorAnalyticsDTO> GetInstructorAnalyticsAsync(
            string instructorId,
            CancellationToken cancellationToken = default);
    }
}
