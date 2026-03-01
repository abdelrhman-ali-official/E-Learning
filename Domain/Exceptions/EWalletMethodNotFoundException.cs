namespace Domain.Exceptions
{
    public sealed class EWalletMethodNotFoundException : NotFoundException
    {
        public EWalletMethodNotFoundException(int methodId)
            : base($"E-wallet payment method with ID {methodId} not found or is inactive")
        {
        }
    }
}
