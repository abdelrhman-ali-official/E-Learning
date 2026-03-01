using FluentValidation;
using Services.Helpers;
using Shared.VideoModels;

namespace Services.Validators;

public class UpdateLiveSessionValidator : AbstractValidator<UpdateLiveSessionDTO>
{
    public UpdateLiveSessionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.YouTubeLiveVideoId)
            .MaximumLength(50).WithMessage("Video ID must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9_-]+$").WithMessage("Invalid YouTube Video ID format")
            .When(x => !string.IsNullOrWhiteSpace(x.YouTubeLiveVideoId));

        RuleFor(x => x.ScheduledEnd)
            .NotEmpty().WithMessage("Scheduled end time is required")
            .GreaterThan(x => x.ScheduledStart).WithMessage("Scheduled end time must be after start time");
    }
}
