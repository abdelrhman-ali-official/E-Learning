namespace Shared.CourseModels
{
    public record CourseReviewDTO
    {
        public int Rating { get; init; } // 1-5 stars
        public string? ReviewText { get; init; }
    }
}
