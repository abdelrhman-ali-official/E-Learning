using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetUserManualPaymentsSpecification : Specifications<ManualPaymentRequest>
    {
        public GetUserManualPaymentsSpecification(string userId) 
            : base(m => m.UserId == userId)
        {
            AddInclude(m => m.Content);
            setOrderByDescending(m => m.CreatedAt);
        }
    }
}
