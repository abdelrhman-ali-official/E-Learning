using FluentValidation;
using Shared.SubscriptionModels;

namespace Services.Validators
{
    public class RejectPaymentValidator : AbstractValidator<RejectPaymentDTO>
    {
        public RejectPaymentValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Rejection reason is required")
                .MaximumLength(500).WithMessage("Rejection reason cannot exceed 500 characters");
        }
    }
}
