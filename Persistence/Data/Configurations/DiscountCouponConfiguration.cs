using Domain.Entities.SubscriptionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class DiscountCouponConfiguration : IEntityTypeConfiguration<DiscountCoupon>
    {
        public void Configure(EntityTypeBuilder<DiscountCoupon> builder)
        {
            builder.ToTable("DiscountCoupons");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.DiscountPercentage)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(c => c.MaxUsage)
                .IsRequired();

            builder.Property(c => c.UsedCount)
                .IsRequired();

            builder.Property(c => c.ExpiryDate)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasMany(c => c.Subscriptions)
                .WithOne(s => s.DiscountCoupon)
                .HasForeignKey(s => s.DiscountCouponId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(c => c.Code)
                .IsUnique();

            builder.HasIndex(c => c.IsActive);
        }
    }
}
