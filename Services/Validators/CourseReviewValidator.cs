using FluentValidation;
using Shared.CourseModels;

namespace Services.Validators
{
    public class CourseReviewValidator : AbstractValidator<CourseReviewDTO>
    {
        public CourseReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars");

            RuleFor(x => x.ReviewText)
                .MaximumLength(2000).WithMessage("Review text cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.ReviewText));
        }
    }
}
