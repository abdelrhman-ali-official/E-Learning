namespace Shared.CourseModels
{
    public record CourseDetailDTO
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? ThumbnailUrl { get; init; }
        public string InstructorId { get; init; } = string.Empty;
        public string InstructorName { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public bool IsFree { get; init; }
        public int AccessDurationDays { get; init; }
        public bool IsPublished { get; init; }
        public bool IsFeatured { get; init; }
        public int EstimatedDurationMinutes { get; init; }
        public string? Category { get; init; }
        public string? Level { get; init; }
        public string[]? Requirements { get; init; } // Parsed from JSON
        public string[]? LearningObjectives { get; init; } // Parsed from JSON
        public int TotalVideos { get; init; }
        public int TotalLiveSessions { get; init; }
        public int TotalEnrollments { get; init; }
        public double AverageRating { get; init; }
        public int TotalReviews { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public DateTime? PublishedAt { get; init; }
        public bool IsEnrolled { get; init; }
    }
}
