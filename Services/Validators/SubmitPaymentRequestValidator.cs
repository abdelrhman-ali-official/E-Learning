using Domain.Entities.SubscriptionEntities;
using FluentValidation;
using Shared.SubscriptionModels;

namespace Services.Validators
{
    public class SubmitPaymentRequestValidator : AbstractValidator<SubmitPaymentRequestDTO>
    {
        public SubmitPaymentRequestValidator()
        {
            RuleFor(x => x.StudentSubscriptionId)
                .NotEmpty().WithMessage("Subscription ID is required");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method");

            RuleFor(x => x.TransactionReference)
                .NotEmpty().WithMessage("Transaction reference is required")
                .MaximumLength(200).WithMessage("Transaction reference cannot exceed 200 characters");

            RuleFor(x => x.PaymentProof)
                .NotNull().WithMessage("Payment proof is required")
                .Must(file => file != null && file.Length > 0)
                .WithMessage("Payment proof file is required")
                .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
                .WithMessage("Payment proof file size cannot exceed 5MB")
                .Must(file => file == null || IsValidImageType(file.ContentType))
                .WithMessage("Only JPG, JPEG, PNG image formats are allowed");
        }

        private bool IsValidImageType(string contentType)
        {
            var validTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
            return validTypes.Contains(contentType.ToLower());
        }
    }
}
