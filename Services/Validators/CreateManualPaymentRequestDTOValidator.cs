using FluentValidation;
using Shared.ContentModels;

namespace Services.Validators
{
    public class CreateManualPaymentRequestDTOValidator : AbstractValidator<CreateManualPaymentRequestDTO>
    {
        public CreateManualPaymentRequestDTOValidator()
        {
            RuleFor(x => x.ContentId)
                .GreaterThan(0).WithMessage("Valid Content ID is required");

            RuleFor(x => x.TransferMethod)
                .InclusiveBetween(1, 2).WithMessage("Transfer method must be InstaPay (1) or VodafoneCash (2)");

            RuleFor(x => x.ReferenceNumber)
                .NotEmpty().WithMessage("Reference number is required")
                .MaximumLength(100).WithMessage("Reference number cannot exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9-_]+$").WithMessage("Reference number can only contain letters, numbers, hyphens, and underscores");
        }
    }
}
