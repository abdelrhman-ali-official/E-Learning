namespace Shared.CourseModels
{
    public record CreateCourseDTO
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? ThumbnailUrl { get; init; }
        public decimal Price { get; init; }
        public bool IsFree { get; init; } = false;
        public int AccessDurationDays { get; init; } = 0;
        public int EstimatedDurationMinutes { get; init; }
        public string? Category { get; init; }
        public string? Level { get; init; } // Beginner, Intermediate, Advanced
        public string? Requirements { get; init; } // JSON string
        public string? LearningObjectives { get; init; } // JSON string
    }
}
