using Domain.Entities.ContentEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(p => p.ContentId)
                .IsRequired();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PurchaseDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.ExpiryDate)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.ManualPaymentRequestId);

            builder.HasOne(p => p.Content)
                .WithMany(c => c.Purchases)
                .HasForeignKey(p => p.ContentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ManualPaymentRequest)
                .WithMany()
                .HasForeignKey(p => p.ManualPaymentRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.ContentId);
            builder.HasIndex(p => new { p.UserId, p.ContentId });
        }
    }
}
