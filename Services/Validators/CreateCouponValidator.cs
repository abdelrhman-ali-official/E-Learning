using FluentValidation;
using Shared.SubscriptionModels;

namespace Services.Validators
{
    public class CreateCouponValidator : AbstractValidator<CreateCouponDTO>
    {
        public CreateCouponValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Coupon code is required")
                .MaximumLength(50).WithMessage("Coupon code cannot exceed 50 characters")
                .Matches("^[A-Z0-9_-]+$").WithMessage("Coupon code must contain only uppercase letters, numbers, hyphens, and underscores");

            RuleFor(x => x.DiscountPercentage)
                .GreaterThan(0).WithMessage("Discount percentage must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Discount percentage cannot exceed 100");

            RuleFor(x => x.MaxUsage)
                .GreaterThan(0).WithMessage("Max usage must be greater than 0");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future");
        }
    }
}
