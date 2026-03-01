using FluentValidation;
using Shared.ContentModels;

namespace Services.Validators
{
    public class ReviewManualPaymentDTOValidator : AbstractValidator<ReviewManualPaymentDTO>
    {
        public ReviewManualPaymentDTOValidator()
        {
            RuleFor(x => x.Status)
                .InclusiveBetween(2, 3).WithMessage("Status must be Approved (2) or Rejected (3)");

            RuleFor(x => x.RejectionReason)
                .NotEmpty().WithMessage("Rejection reason is required when rejecting a payment")
                .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters")
                .When(x => x.Status == 3);
        }
    }
}
