using Domain.Entities.ContentEntities;
using Domain.Exceptions;
using Services.Specifications;
using Shared;
using Shared.ContentModels;

namespace Services
{
    public class PurchaseService(IUnitOFWork unitOfWork, IMapper mapper) : IPurchaseService
    {
        public async Task<IEnumerable<PurchaseResultDTO>> GetUserPurchasesAsync(string userId)
        {
            var purchases = await unitOfWork.GetRepository<Purchase, int>()
                .GetAllAsync(new GetActiveUserPurchasesSpecification(userId));

            return mapper.Map<IEnumerable<PurchaseResultDTO>>(purchases);
        }

        public async Task<PaginatedResult<PurchaseResultDTO>> GetAllPurchasesAsync(int pageIndex = 1, int pageSize = 10)
        {
            var spec = new GetAllPurchasesSpecification(pageIndex, pageSize);
            var (purchases, totalCount) = await unitOfWork.GetRepository<Purchase, int>()
                .GetPagedAsync(spec, pageIndex, pageSize);

            var purchaseDtos = mapper.Map<IEnumerable<PurchaseResultDTO>>(purchases);

            return new PaginatedResult<PurchaseResultDTO>(
                pageIndex,
                purchaseDtos.Count(),
                totalCount,
                purchaseDtos);
        }

        public async Task DeactivateExpiredPurchasesAsync()
        {
            var expiredPurchases = await unitOfWork.GetRepository<Purchase, int>()
                .GetAllAsync(new GetExpiredPurchasesSpecification());

            foreach (var purchase in expiredPurchases)
            {
                purchase.IsActive = false;
                unitOfWork.GetRepository<Purchase, int>().Update(purchase);
            }

            if (expiredPurchases.Any())
            {
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
