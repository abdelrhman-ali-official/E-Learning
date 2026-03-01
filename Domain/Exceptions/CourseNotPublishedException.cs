namespace Domain.Exceptions
{
    public class CourseNotPublishedException : Exception
    {
        public CourseNotPublishedException(Guid courseId)
            : base($"Course with ID '{courseId}' is not published.")
        {
        }
    }
}
