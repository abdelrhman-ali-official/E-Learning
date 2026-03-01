using Shared;
using Shared.ContentModels;

namespace Services.Abstractions
{
    public interface IContentService
    {
        // --- Instructor / course-scoped ---
        Task<ContentResultDTO> CreateCourseContentAsync(Guid courseId, string instructorId, CreateContentDTO dto);
        Task<ContentResultDTO> UpdateCourseContentAsync(int id, string instructorId, UpdateContentDTO dto);
        Task DeleteCourseContentAsync(int id, string instructorId);
        Task<IEnumerable<ContentResultDTO>> GetCourseContentsAsync(Guid courseId);
        Task<ContentResultDTO> GetCourseContentByIdAsync(Guid courseId, int id);

        // --- Public (no enrollment required) ---
        /// <summary>
        /// Returns visible content items for a published course.
        /// Only Title, Type, Description and IsDownloadable flag are exposed — no media URLs.
        /// </summary>
        Task<IEnumerable<PublicCourseContentDTO>> GetPublicCourseContentsAsync(Guid courseId);

        // --- Student completion tracking ---
        /// <summary>Marks a content item complete or incomplete for a student, and recalculates enrollment progress.</summary>
        Task MarkContentCompleteAsync(Guid courseId, int contentId, string userId, bool isComplete);

        /// <summary>Returns per-item completion status and overall progress for a student in a course.</summary>
        Task<ContentProgressDTO> GetCourseContentProgressAsync(Guid courseId, string userId);

        // --- Admin management ---
        Task<PaginatedResult<ContentResultDTO>> GetAllContentsAsync(int pageIndex = 1, int pageSize = 10);
        Task DeleteContentAsync(int id);
        Task<ContentResultDTO> ToggleVisibilityAsync(int id);
    }
}
