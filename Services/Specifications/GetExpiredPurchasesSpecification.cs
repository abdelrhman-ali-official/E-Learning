using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetExpiredPurchasesSpecification : Specifications<Purchase>
    {
        public GetExpiredPurchasesSpecification() 
            : base(p => p.IsActive && p.ExpiryDate <= DateTime.UtcNow)
        {
        }
    }
}
