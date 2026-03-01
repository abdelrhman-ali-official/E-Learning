using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetPendingUserManualPaymentForContentSpecification : Specifications<ManualPaymentRequest>
    {
        public GetPendingUserManualPaymentForContentSpecification(string userId, int contentId) 
            : base(r => r.UserId == userId && 
                       r.ContentId == contentId && 
                       r.Status == ManualPaymentStatus.Pending)
        {
            AddInclude(r => r.Content);
        }
    }
}
