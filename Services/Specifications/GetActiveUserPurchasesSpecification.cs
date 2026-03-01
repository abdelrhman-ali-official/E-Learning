using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetActiveUserPurchasesSpecification : Specifications<Purchase>
    {
        public GetActiveUserPurchasesSpecification(string userId) 
            : base(p => p.UserId == userId && p.IsActive && p.ExpiryDate > DateTime.UtcNow)
        {
            AddInclude(p => p.Content);
            setOrderByDescending(p => p.PurchaseDate);
        }
    }
}
