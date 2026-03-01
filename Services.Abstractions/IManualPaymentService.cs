using Shared;
using Shared.ContentModels;

namespace Services.Abstractions
{
    public interface IManualPaymentService
    {
        Task<ManualPaymentRequestResultDTO> CreateManualPaymentRequestAsync(CreateManualPaymentRequestDTO dto, string userId, string screenshotUrl);
        Task<ManualPaymentRequestResultDTO> ReviewManualPaymentAsync(int id, ReviewManualPaymentDTO dto, string reviewedBy);
        Task<PaginatedResult<ManualPaymentRequestResultDTO>> GetPendingRequestsAsync(int pageIndex = 1, int pageSize = 10);
        Task<IEnumerable<ManualPaymentRequestResultDTO>> GetUserRequestsAsync(string userId);
    }
}
