using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetPendingManualPaymentsSpecification : Specifications<ManualPaymentRequest>
    {
        public GetPendingManualPaymentsSpecification(int? pageIndex = null, int? pageSize = null) 
            : base(m => m.Status == ManualPaymentStatus.Pending)
        {
            AddInclude(m => m.Content);
            setOrderBy(m => m.CreatedAt);
            
            if (pageIndex.HasValue && pageSize.HasValue && pageIndex > 0 && pageSize > 0)
            {
                ApplyPagination(pageIndex.Value, pageSize.Value);
            }
        }
    }
}
