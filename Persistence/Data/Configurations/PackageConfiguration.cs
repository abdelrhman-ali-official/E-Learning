using Domain.Entities.SubscriptionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.ToTable("Packages");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.PriceMonthly)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PriceYearly)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.DiscountPercentage)
                .HasColumnType("decimal(5,2)");

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.Property(p => p.IsFeatured)
                .IsRequired();

            builder.Property(p => p.MaxCoursesAccess)
                .IsRequired();

            builder.Property(p => p.HasLiveAccess)
                .IsRequired();

            builder.Property(p => p.HasRecordedAccess)
                .IsRequired();

            builder.Property(p => p.HasCertificate)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            builder.HasMany(p => p.Features)
                .WithOne(f => f.Package)
                .HasForeignKey(f => f.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Subscriptions)
                .WithOne(s => s.Package)
                .HasForeignKey(s => s.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.IsActive);
            builder.HasIndex(p => p.IsFeatured);
        }
    }
}
