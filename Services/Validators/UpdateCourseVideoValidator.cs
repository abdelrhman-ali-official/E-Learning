using FluentValidation;
using Shared.VideoModels;

namespace Services.Validators;

public class UpdateCourseVideoValidator : AbstractValidator<UpdateCourseVideoDTO>
{
    public UpdateCourseVideoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.VideoId)
            .NotEmpty().WithMessage("YouTube Video ID is required")
            .MaximumLength(50).WithMessage("Video ID must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Invalid YouTube Video ID format");

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage("Duration must be greater than 0 seconds");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index must be 0 or greater");
    }
}
