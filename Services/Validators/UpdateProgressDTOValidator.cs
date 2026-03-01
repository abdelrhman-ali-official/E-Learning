using FluentValidation;
using Shared.ContentModels;

namespace Services.Validators
{
    public class UpdateProgressDTOValidator : AbstractValidator<UpdateProgressDTO>
    {
        public UpdateProgressDTOValidator()
        {
            RuleFor(x => x.LastPositionSeconds)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Last position cannot be negative");
        }
    }
}
