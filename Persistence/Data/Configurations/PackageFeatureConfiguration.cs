using Domain.Entities.SubscriptionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PackageFeatureConfiguration : IEntityTypeConfiguration<PackageFeature>
    {
        public void Configure(EntityTypeBuilder<PackageFeature> builder)
        {
            builder.ToTable("PackageFeatures");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(f => f.PackageId)
                .IsRequired();

            builder.HasOne(f => f.Package)
                .WithMany(p => p.Features)
                .HasForeignKey(f => f.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
