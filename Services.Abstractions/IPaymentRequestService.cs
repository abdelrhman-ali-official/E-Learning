using Shared.SubscriptionModels;

namespace Services.Abstractions
{
    public interface IPaymentRequestService
    {
        Task<PaymentResponseDTO> SubmitPaymentRequestAsync(Guid studentId, SubmitPaymentRequestDTO dto, CancellationToken cancellationToken = default);
        Task<PaymentResponseDTO> ApprovePaymentAsync(Guid paymentRequestId, Guid adminId, ApprovePaymentDTO dto, CancellationToken cancellationToken = default);
        Task<PaymentResponseDTO> RejectPaymentAsync(Guid paymentRequestId, Guid adminId, RejectPaymentDTO dto, CancellationToken cancellationToken = default);
        Task<IEnumerable<PaymentResponseDTO>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PaymentResponseDTO>> GetPaymentsBySubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default);
    }
}
