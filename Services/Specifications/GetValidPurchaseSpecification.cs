using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetValidPurchaseSpecification : Specifications<Purchase>
    {
        public GetValidPurchaseSpecification(string userId, int contentId) 
            : base(p => p.UserId == userId 
                     && p.ContentId == contentId 
                     && p.IsActive 
                     && p.ExpiryDate > DateTime.UtcNow)
        {
            AddInclude(p => p.Content);
        }
    }
}
