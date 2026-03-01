namespace Shared.CourseModels
{
    public record EnrollmentResponseDTO
    {
        public Guid Id { get; init; }
        public Guid CourseId { get; init; }
        public string CourseTitle { get; init; } = string.Empty;
        public string StudentId { get; init; } = string.Empty;
        public string EnrollmentSource { get; init; } = string.Empty;
        public DateTime EnrolledAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public bool IsActive { get; init; }
        public int ProgressPercentage { get; init; }
        public int CompletedVideos { get; init; }
        public int TotalVideos { get; init; }
        public DateTime LastAccessedAt { get; init; }
        public bool IsCertificateIssued { get; init; }
    }
}
