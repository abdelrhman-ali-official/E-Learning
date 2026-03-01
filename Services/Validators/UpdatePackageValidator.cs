using FluentValidation;
using Shared.SubscriptionModels;

namespace Services.Validators
{
    public class UpdatePackageValidator : AbstractValidator<UpdatePackageDTO>
    {
        public UpdatePackageValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Package name is required")
                .MaximumLength(200).WithMessage("Package name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.PriceMonthly)
                .GreaterThan(0).WithMessage("Monthly price must be greater than 0");

            RuleFor(x => x.PriceYearly)
                .GreaterThan(0).WithMessage("Yearly price must be greater than 0");

            RuleFor(x => x.DiscountPercentage)
                .GreaterThanOrEqualTo(0).WithMessage("Discount percentage cannot be negative")
                .LessThanOrEqualTo(100).WithMessage("Discount percentage cannot exceed 100");

            RuleFor(x => x.MaxCoursesAccess)
                .GreaterThan(0).WithMessage("Max courses access must be greater than 0");

            RuleForEach(x => x.Features).ChildRules(feature =>
            {
                feature.RuleFor(f => f.Title)
                    .NotEmpty().WithMessage("Feature title is required")
                    .MaximumLength(200).WithMessage("Feature title cannot exceed 200 characters");

                feature.RuleFor(f => f.Description)
                    .NotEmpty().WithMessage("Feature description is required")
                    .MaximumLength(500).WithMessage("Feature description cannot exceed 500 characters");
            });
        }
    }
}
