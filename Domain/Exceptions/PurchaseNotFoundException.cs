namespace Domain.Exceptions
{
    public sealed class PurchaseNotFoundException : NotFoundException
    {
        public PurchaseNotFoundException(string sessionId) 
            : base($"Purchase with session ID {sessionId} not found")
        {
        }
    }
}
