using FluentValidation;
using Shared.VideoModels;

namespace Services.Validators;

public class UpdateVideoProgressValidator : AbstractValidator<UpdateVideoProgressDTO>
{
    public UpdateVideoProgressValidator()
    {
        RuleFor(x => x.WatchedSeconds)
            .GreaterThanOrEqualTo(0).WithMessage("Watched seconds must be 0 or greater");
    }
}
