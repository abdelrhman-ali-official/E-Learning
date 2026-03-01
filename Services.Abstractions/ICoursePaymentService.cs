using Shared;
using Shared.CourseModels;

namespace Services.Abstractions
{
    public interface ICoursePaymentService
    {
        // ── Instructor: manage e-wallet methods ──────────────────────────────────
        Task<EWalletMethodDTO> AddEWalletMethodAsync(Guid courseId, string instructorId, AddEWalletMethodDTO dto);
        Task<EWalletMethodDTO> UpdateEWalletMethodAsync(int methodId, string instructorId, AddEWalletMethodDTO dto);
        Task RemoveEWalletMethodAsync(int methodId, string instructorId);
        Task<IEnumerable<EWalletMethodDTO>> GetCourseEWalletMethodsAsync(Guid courseId);

        // ── Instructor: view & review student payments ──────────────────────────────
        Task<PaginatedResult<CoursePaymentRequestDTO>> GetCoursePaymentRequestsAsync(Guid courseId, string instructorId, string? status, int pageIndex, int pageSize);
        Task<CoursePaymentRequestDTO> GetCoursePaymentRequestByIdAsync(int requestId, string instructorId);
        Task<CoursePaymentRequestDTO> InstructorReviewPaymentRequestAsync(int requestId, string instructorId, ReviewCoursePaymentDTO dto);

        // ── Student: pay for a course ─────────────────────────────────────────────
        Task<CoursePaymentRequestDTO> SubmitPaymentRequestAsync(Guid courseId, string studentId, SubmitCoursePaymentDTO dto, string screenshotUrl);
        Task<IEnumerable<CoursePaymentRequestDTO>> GetMyPaymentRequestsAsync(string studentId);

        // ── Admin: review payments ────────────────────────────────────────────────
        Task<CoursePaymentRequestDTO> ReviewPaymentRequestAsync(int requestId, string reviewerId, ReviewCoursePaymentDTO dto);
        Task<PaginatedResult<CoursePaymentRequestDTO>> GetPendingPaymentRequestsAsync(int pageIndex = 1, int pageSize = 20);
        Task<PaginatedResult<CoursePaymentRequestDTO>> GetAllPaymentRequestsAsync(int pageIndex = 1, int pageSize = 20);
    }
}
