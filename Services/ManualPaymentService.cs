using Domain.Entities.ContentEntities;
using Domain.Exceptions;
using Services.Specifications;
using Shared;
using Shared.ContentModels;

namespace Services
{
    public class ManualPaymentService(IUnitOFWork unitOfWork, IMapper mapper) : IManualPaymentService
    {
        public async Task<ManualPaymentRequestResultDTO> CreateManualPaymentRequestAsync(
            CreateManualPaymentRequestDTO dto, 
            string userId,
            string screenshotUrl)
        {
            var content = await unitOfWork.GetRepository<Content, int>()
                .GetAsync(new GetContentByIdSpecification(dto.ContentId))
                ?? throw new ContentNotFoundException(dto.ContentId);

            if (!content.IsVisible)
                throw new ValidationException(new[] { "This content is not available for purchase" });

            // Check if user already has active purchase
            var existingPurchase = await unitOfWork.GetRepository<Purchase, int>()
                .GetAsync(new GetValidPurchaseSpecification(userId, dto.ContentId));

            if (existingPurchase != null)
                throw new ValidationException(new[] { "You already have active access to this content" });

            // Check if user already has pending payment request for this content
            var existingPendingRequest = await unitOfWork.GetRepository<ManualPaymentRequest, int>()
                .GetAsync(new GetPendingUserManualPaymentForContentSpecification(userId, dto.ContentId));

            if (existingPendingRequest != null)
                throw new ValidationException(new[] { "You already have a pending payment request for this content" });

            var request = new ManualPaymentRequest
            {
                UserId = userId,
                ContentId = dto.ContentId,
                TransferMethod = (PaymentMethod)dto.TransferMethod,
                ReferenceNumber = dto.ReferenceNumber,
                ScreenshotUrl = screenshotUrl,
                Amount = content.Price,
                Status = ManualPaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.GetRepository<ManualPaymentRequest, int>().AddAsync(request);
            await unitOfWork.SaveChangesAsync();

            // Reload with content navigation property
            var createdRequest = await unitOfWork.GetRepository<ManualPaymentRequest, int>()
                .GetAsync(new GetManualPaymentRequestByIdSpecification(request.Id));

            return mapper.Map<ManualPaymentRequestResultDTO>(createdRequest);
        }

        public async Task<ManualPaymentRequestResultDTO> ReviewManualPaymentAsync(
            int id, 
            ReviewManualPaymentDTO dto, 
            string reviewedBy)
        {
            var request = await unitOfWork.GetRepository<ManualPaymentRequest, int>()
                .GetAsync(new GetManualPaymentRequestByIdSpecification(id))
                ?? throw new ManualPaymentRequestNotFoundException(id);

            if (request.Status != ManualPaymentStatus.Pending)
                throw new ValidationException(new[] { "This request has already been reviewed" });

            request.Status = (ManualPaymentStatus)dto.Status;
            request.ReviewedAt = DateTime.UtcNow;
            request.ReviewedBy = reviewedBy;
            request.RejectionReason = dto.RejectionReason;

            unitOfWork.GetRepository<ManualPaymentRequest, int>().Update(request);

            // If approved, create purchase
            if (request.Status == ManualPaymentStatus.Approved)
            {
                var content = await unitOfWork.GetRepository<Content, int>()
                    .GetAsync(new GetContentByIdSpecification(request.ContentId))
                    ?? throw new ContentNotFoundException(request.ContentId);

                var purchase = new Purchase
                {
                    UserId = request.UserId,
                    ContentId = request.ContentId,
                    Amount = request.Amount,
                    PurchaseDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(content.AccessDurationWeeks * 7),
                    IsActive = true
                };

                await unitOfWork.GetRepository<Purchase, int>().AddAsync(purchase);
            }

            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ManualPaymentRequestResultDTO>(request);
        }

        public async Task<PaginatedResult<ManualPaymentRequestResultDTO>> GetPendingRequestsAsync(
            int pageIndex = 1, 
            int pageSize = 10)
        {
            var spec = new GetPendingManualPaymentsSpecification(pageIndex, pageSize);
            var (requests, totalCount) = await unitOfWork.GetRepository<ManualPaymentRequest, int>()
                .GetPagedAsync(spec, pageIndex, pageSize);

            var requestDtos = mapper.Map<IEnumerable<ManualPaymentRequestResultDTO>>(requests);

            return new PaginatedResult<ManualPaymentRequestResultDTO>(
                pageIndex,
                requestDtos.Count(),
                totalCount,
                requestDtos);
        }

        public async Task<IEnumerable<ManualPaymentRequestResultDTO>> GetUserRequestsAsync(string userId)
        {
            var requests = await unitOfWork.GetRepository<ManualPaymentRequest, int>()
                .GetAllAsync(new GetUserManualPaymentsSpecification(userId));

            return mapper.Map<IEnumerable<ManualPaymentRequestResultDTO>>(requests);
        }
    }
}
