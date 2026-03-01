using Domain.Exceptions;

namespace Domain.Exceptions
{
    public class CourseNotFoundException : NotFoundException
    {
        public CourseNotFoundException(Guid courseId)
            : base($"Course with ID '{courseId}' was not found.")
        {
        }
        
        public CourseNotFoundException(string slug)
            : base($"Course with slug '{slug}' was not found.")
        {
        }
    }
}
