using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetAllPurchasesSpecification : Specifications<Purchase>
    {
        public GetAllPurchasesSpecification(int? pageIndex = null, int? pageSize = null)
        {
            AddInclude(p => p.Content);
            setOrderByDescending(p => p.PurchaseDate);
            
            if (pageIndex.HasValue && pageSize.HasValue && pageIndex > 0 && pageSize > 0)
            {
                ApplyPagination(pageIndex.Value, pageSize.Value);
            }
        }
    }
}
