using Shared;
using Shared.ContentModels;

namespace Services.Abstractions
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseResultDTO>> GetUserPurchasesAsync(string userId);
        Task<PaginatedResult<PurchaseResultDTO>> GetAllPurchasesAsync(int pageIndex = 1, int pageSize = 10);
        Task DeactivateExpiredPurchasesAsync();
    }
}
