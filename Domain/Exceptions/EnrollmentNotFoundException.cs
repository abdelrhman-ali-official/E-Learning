using Domain.Exceptions;

namespace Domain.Exceptions
{
    public class EnrollmentNotFoundException : NotFoundException
    {
        public EnrollmentNotFoundException(Guid enrollmentId)
            : base($"Enrollment with ID '{enrollmentId}' was not found.")
        {
        }
    }
}
