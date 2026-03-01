namespace Shared.SecurityModels
{
    /// <summary>
    /// Student record returned to an instructor.
    /// </summary>
    public record StudentSummaryDTO
    {
        public string Id { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        /// <summary>Number of the instructor's courses this student is actively enrolled in.</summary>
        public int EnrolledInMyCoursesCount { get; init; }
    }
}
