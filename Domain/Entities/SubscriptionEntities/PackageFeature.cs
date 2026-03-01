namespace Domain.Entities.SubscriptionEntities
{
    public class PackageFeature : BaseEntity<Guid>
    {
        public Guid PackageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Package Package { get; set; } = null!;
    }
}
