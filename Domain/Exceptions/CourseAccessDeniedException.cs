namespace Domain.Exceptions;

public sealed class CourseAccessDeniedException : Exception
{
    public CourseAccessDeniedException(Guid courseId)
        : base($"Access denied to course {courseId}. Purchase required or subscription expired.")
    {
    }

    public CourseAccessDeniedException(string message)
        : base(message)
    {
    }
}
