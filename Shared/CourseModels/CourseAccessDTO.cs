namespace Shared.CourseModels
{
    public record CourseAccessDTO
    {
        public bool HasAccess { get; init; }
        public string? AccessReason { get; init; } // "Enrollment", "Subscription", "Free", etc.
        public DateTime? ExpiresAt { get; init; }
        public string? DeniedReason { get; init; } // If HasAccess = false
    }
}
