namespace Shared.CourseModels
{
    public record UpdateCourseDTO
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public string? ThumbnailUrl { get; init; }
        public decimal? Price { get; init; }
        public bool? IsFree { get; init; }
        public int? AccessDurationDays { get; init; }
        public int? EstimatedDurationMinutes { get; init; }
        public string? Category { get; init; }
        public string? Level { get; init; }
        public string? Requirements { get; init; }
        public string? LearningObjectives { get; init; }
    }
}
