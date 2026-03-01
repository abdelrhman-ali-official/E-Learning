using Domain.Exceptions;

namespace Domain.Exceptions
{
    public class PaymentRequestNotFoundException : NotFoundException
    {
        public PaymentRequestNotFoundException(Guid id)
            : base($"Payment request with ID '{id}' was not found")
        {
        }
    }
}
