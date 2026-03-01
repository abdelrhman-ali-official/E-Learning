using Shared;
using Shared.CourseModels;

namespace Services.Abstractions
{
    public interface ICourseService
    {
        // Admin CRUD
        Task<CourseResponseDTO> CreateCourseAsync(CreateCourseDTO dto, string instructorId, string instructorName);
        Task<CourseResponseDTO> UpdateCourseAsync(Guid id, UpdateCourseDTO dto);
        Task DeleteCourseAsync(Guid id); // Soft delete
        Task<CourseDetailDTO> GetCourseByIdAsync(Guid id);

        Task<PaginatedResult<CourseResponseDTO>> GetAllCoursesAsync(int pageIndex = 1, int pageSize = 10, bool includeUnpublished = false);
        
        // Instructor-scoped
        /// <summary>Update price and access duration. Only the owning instructor may call this.</summary>
        Task<CourseResponseDTO> UpdateCoursePricingAsync(Guid courseId, string instructorId, UpdateCoursePricingDTO dto);
        Task<PaginatedResult<CourseResponseDTO>> GetInstructorCoursesAsync(string instructorId, int pageIndex = 1, int pageSize = 10);

        // Publishing
        Task PublishCourseAsync(Guid id);
        Task UnpublishCourseAsync(Guid id);
        Task ToggleFeatureAsync(Guid id);
        
        // Student access
        Task<CourseAccessDTO> CheckCourseAccessAsync(Guid courseId, string userId);
        Task<PaginatedResult<CourseResponseDTO>> GetPublishedCoursesAsync(int pageIndex = 1, int pageSize = 10, string? category = null, string? level = null, string? studentId = null);
        Task<CourseDetailDTO> GetCourseBySlugAsync(string slug, string? studentId = null);
        
        // Reviews
        Task<CourseReviewResponseDTO> SubmitReviewAsync(Guid courseId, string userId, string userName, CourseReviewDTO dto);
        Task<IEnumerable<CourseReviewResponseDTO>> GetCourseReviewsAsync(Guid courseId);
    }
}
