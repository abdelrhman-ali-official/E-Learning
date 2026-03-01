using FluentValidation;
using Shared.ContentModels;

namespace Services.Validators
{
    public class CreateContentDTOValidator : AbstractValidator<CreateContentDTO>
    {
        public CreateContentDTOValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Type)
                .InclusiveBetween(1, 7).WithMessage("Type must be between 1 (Video) and 7 (Other)");

            RuleFor(x => x.MediaLink)
                .MaximumLength(1000).WithMessage("MediaLink cannot exceed 1000 characters")
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(
                    url.StartsWith("www.", StringComparison.OrdinalIgnoreCase) ? "https://" + url : url,
                    UriKind.Absolute))
                .WithMessage("MediaLink must be a valid URL")
                .When(x => !string.IsNullOrEmpty(x.MediaLink));
        }
    }
}
