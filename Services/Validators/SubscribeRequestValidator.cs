using Domain.Entities.SubscriptionEntities;
using FluentValidation;
using Shared.SubscriptionModels;

namespace Services.Validators
{
    public class SubscribeRequestValidator : AbstractValidator<SubscribeRequestDTO>
    {
        public SubscribeRequestValidator()
        {
            RuleFor(x => x.PackageId)
                .NotEmpty().WithMessage("Package ID is required");

            RuleFor(x => x.BillingCycle)
                .IsInEnum().WithMessage("Invalid billing cycle")
                .Must(x => x == BillingCycle.Monthly || x == BillingCycle.Yearly)
                .WithMessage("Billing cycle must be Monthly or Yearly");

            RuleFor(x => x.CouponCode)
                .MaximumLength(50).WithMessage("Coupon code cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.CouponCode));
        }
    }
}
