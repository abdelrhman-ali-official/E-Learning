using AutoMapper;
using Domain.Contracts;
using Domain.Entities.SubscriptionEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.SubscriptionModels;

namespace Services
{
    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly ICouponService _couponService;

        public PaymentRequestService(IUnitOFWork unitOfWork, IMapper mapper, IStorageService storageService, ICouponService couponService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _storageService = storageService;
            _couponService = couponService;
        }

        public async Task<PaymentResponseDTO> SubmitPaymentRequestAsync(Guid studentId, SubmitPaymentRequestDTO dto, CancellationToken cancellationToken = default)
        {
            // Verify subscription exists and belongs to the student
            var subscription = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAsync(dto.StudentSubscriptionId);
            if (subscription == null)
                throw new SubscriptionNotFoundException(dto.StudentSubscriptionId);

            if (subscription.StudentId != studentId.ToString())
                throw new UnAuthorizedException("You can only submit payments for your own subscriptions");

            // Check if there's already a pending payment request for this subscription
            var paymentRequests = await _unitOfWork.GetRepository<PaymentRequest, Guid>().GetAllAsync();
            var pendingPayment = paymentRequests.FirstOrDefault(pr => 
                pr.StudentSubscriptionId == dto.StudentSubscriptionId && 
                pr.Status == PaymentStatus.Pending);

            if (pendingPayment != null)
                throw new ValidationException(new[] { "You already have a pending payment request for this subscription" });

            // Upload payment proof to storage
            using var stream = dto.PaymentProof.OpenReadStream();
            var paymentProofUrl = await _storageService.UploadFileAsync(stream, dto.PaymentProof.FileName, dto.PaymentProof.ContentType);

            // Create payment request
            var paymentRequest = new PaymentRequest
            {
                StudentSubscriptionId = dto.StudentSubscriptionId,
                StudentId = studentId.ToString(),
                Amount = subscription.FinalPrice,
                PaymentMethod = dto.PaymentMethod,
                TransactionReference = dto.TransactionReference,
                PaymentProofUrl = paymentProofUrl,
                Status = PaymentStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<PaymentRequest, Guid>().AddAsync(paymentRequest);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentResponseDTO>(paymentRequest);
        }

        public async Task<PaymentResponseDTO> ApprovePaymentAsync(Guid paymentRequestId, Guid adminId, ApprovePaymentDTO dto, CancellationToken cancellationToken = default)
        {
            var paymentRequest = await _unitOfWork.GetRepository<PaymentRequest, Guid>().GetAsync(paymentRequestId);
            if (paymentRequest == null)
                throw new PaymentRequestNotFoundException(paymentRequestId);

            if (paymentRequest.Status != PaymentStatus.Pending)
                throw new ValidationException(new[] { "Only pending payment requests can be approved" });

            // Update payment request
            paymentRequest.Status = PaymentStatus.Approved;
            paymentRequest.AdminNotes = dto.AdminNotes;
            paymentRequest.ReviewedAt = DateTime.UtcNow;
            paymentRequest.ReviewedBy = adminId.ToString();

            // Activate subscription
            var subscription = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAsync(paymentRequest.StudentSubscriptionId);
            if (subscription != null)
            {
                subscription.Status = SubscriptionStatus.Active;
                
                // Increment coupon usage if coupon was used
                if (subscription.DiscountCouponId.HasValue)
                {
                    await _couponService.IncrementCouponUsageAsync(subscription.DiscountCouponId.Value, cancellationToken);
                }
                
                _unitOfWork.GetRepository<StudentSubscription, Guid>().Update(subscription);
            }

            _unitOfWork.GetRepository<PaymentRequest, Guid>().Update(paymentRequest);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentResponseDTO>(paymentRequest);
        }

        public async Task<PaymentResponseDTO> RejectPaymentAsync(Guid paymentRequestId, Guid adminId, RejectPaymentDTO dto, CancellationToken cancellationToken = default)
        {
            var paymentRequest = await _unitOfWork.GetRepository<PaymentRequest, Guid>().GetAsync(paymentRequestId);
            if (paymentRequest == null)
                throw new PaymentRequestNotFoundException(paymentRequestId);

            if (paymentRequest.Status != PaymentStatus.Pending)
                throw new ValidationException(new[] { "Only pending payment requests can be rejected" });

            paymentRequest.Status = PaymentStatus.Rejected;
            paymentRequest.AdminNotes = dto.Reason;
            paymentRequest.ReviewedAt = DateTime.UtcNow;
            paymentRequest.ReviewedBy = adminId.ToString();

            _unitOfWork.GetRepository<PaymentRequest, Guid>().Update(paymentRequest);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentResponseDTO>(paymentRequest);
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetPendingPaymentsAsync(CancellationToken cancellationToken = default)
        {
            var paymentRequests = await _unitOfWork.GetRepository<PaymentRequest, Guid>().GetAllAsync();
            var pendingPayments = paymentRequests.Where(pr => pr.Status == PaymentStatus.Pending)
                                                  .OrderBy(pr => pr.RequestedAt);

            return _mapper.Map<IEnumerable<PaymentResponseDTO>>(pendingPayments);
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetPaymentsBySubscriptionAsync(Guid subscriptionId, CancellationToken cancellationToken = default)
        {
            var paymentRequests = await _unitOfWork.GetRepository<PaymentRequest, Guid>().GetAllAsync();
            var subscriptionPayments = paymentRequests.Where(pr => pr.StudentSubscriptionId == subscriptionId);

            return _mapper.Map<IEnumerable<PaymentResponseDTO>>(subscriptionPayments);
        }
    }
}
