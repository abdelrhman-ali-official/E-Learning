using Domain.Entities.SubscriptionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class StudentSubscriptionConfiguration : IEntityTypeConfiguration<StudentSubscription>
    {
        public void Configure(EntityTypeBuilder<StudentSubscription> builder)
        {
            builder.ToTable("StudentSubscriptions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.StudentId)
                .IsRequired();

            builder.Property(s => s.PackageId)
                .IsRequired();

            builder.Property(s => s.StartDate)
                .IsRequired();

            builder.Property(s => s.EndDate)
                .IsRequired();

            builder.Property(s => s.BillingCycle)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.FinalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Package)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.DiscountCoupon)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.DiscountCouponId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasMany(s => s.PaymentRequests)
                .WithOne(pr => pr.StudentSubscription)
                .HasForeignKey(pr => pr.StudentSubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.StudentId);
            builder.HasIndex(s => s.Status);
            builder.HasIndex(s => new { s.StudentId, s.Status });
        }
    }
}
