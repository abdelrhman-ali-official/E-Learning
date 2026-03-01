namespace Shared.CourseModels
{
    public record CourseReviewResponseDTO
    {
        public Guid Id { get; init; }
        public Guid CourseId { get; init; }
        public string StudentId { get; init; } = string.Empty;
        public string StudentName { get; init; } = string.Empty;
        public int Rating { get; init; }
        public string? ReviewText { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
