namespace Domain.Exceptions
{
    public class PackageNotFoundException : Exception
    {
        public PackageNotFoundException(Guid packageId)
            : base($"Package with ID '{packageId}' was not found.")
        {
        }
    }
}
