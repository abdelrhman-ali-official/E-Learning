using Shared.CourseModels;

namespace Services.Abstractions
{
    public interface IEnrollmentService
    {
        // Enrollment management
        Task<EnrollmentResponseDTO> EnrollStudentAsync(Guid courseId, string studentId, string studentName);
        Task<EnrollmentResponseDTO?> GetEnrollmentAsync(Guid courseId, string studentId);
        Task<IEnumerable<EnrollmentResponseDTO>> GetStudentEnrollmentsAsync(string studentId);
        Task UpdateProgressAsync(Guid enrollmentId, int completedVideos, int totalVideos);
        Task IssueCertificateAsync(Guid enrollmentId);
        
        // Access validation
        Task<bool> HasAccessToCourseAsync(Guid courseId, string studentId);
    }
}
