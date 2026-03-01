using Domain.Entities.SubscriptionEntities;
using Microsoft.AspNetCore.Http;

namespace Shared.SubscriptionModels
{
    public class SubmitPaymentRequestDTO
    {
        public Guid StudentSubscriptionId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public IFormFile PaymentProof { get; set; } = null!;
    }
}
