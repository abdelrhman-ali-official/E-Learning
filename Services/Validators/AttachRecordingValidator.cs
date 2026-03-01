using FluentValidation;
using Services.Helpers;
using Shared.VideoModels;

namespace Services.Validators;

public class AttachRecordingValidator : AbstractValidator<AttachRecordingDTO>
{
    public AttachRecordingValidator()
    {
        RuleFor(x => x.RecordingLink)
            .NotEmpty().WithMessage("Recording link is required")
            .MaximumLength(500).WithMessage("Recording link must not exceed 500 characters")
            .Must(UrlHelper.IsValidUrl).WithMessage("Recording link must be a valid URL");
    }
}
