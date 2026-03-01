using FluentValidation;
using Services.Helpers;
using Shared.VideoModels;

namespace Services.Validators;

public class CreateLiveSessionValidator : AbstractValidator<CreateLiveSessionDTO>
{
    public CreateLiveSessionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.ScheduledStart)
            .NotEmpty().WithMessage("Scheduled start time is required")
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5)).WithMessage("Scheduled start time must be in the future or within last 5 minutes");

        RuleFor(x => x.ScheduledEnd)
            .NotEmpty().WithMessage("Scheduled end time is required")
            .GreaterThan(x => x.ScheduledStart).WithMessage("Scheduled end time must be after start time");
    }
}
