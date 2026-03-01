using Domain.Contracts;
using Domain.Entities.ContentEntities;

namespace Services.Specifications
{
    public class GetManualPaymentRequestByIdSpecification : Specifications<ManualPaymentRequest>
    {
        public GetManualPaymentRequestByIdSpecification(int requestId) 
            : base(m => m.Id == requestId)
        {
            AddInclude(m => m.Content);
        }
    }
}
