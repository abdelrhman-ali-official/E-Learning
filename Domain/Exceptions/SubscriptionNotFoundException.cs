namespace Domain.Exceptions
{
    public class SubscriptionNotFoundException : Exception
    {
        public SubscriptionNotFoundException(Guid subscriptionId)
            : base($"Subscription with ID '{subscriptionId}' was not found.")
        {
        }
    }
}
