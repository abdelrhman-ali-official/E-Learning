namespace Domain.Exceptions
{
    public sealed class ManualPaymentRequestNotFoundException : NotFoundException
    {
        public ManualPaymentRequestNotFoundException(int requestId) 
            : base($"Manual payment request with ID {requestId} not found")
        {
        }
    }
}
