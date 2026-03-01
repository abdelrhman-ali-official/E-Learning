using Domain.Entities.ContentEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class ManualPaymentRequestConfiguration : IEntityTypeConfiguration<ManualPaymentRequest>
    {
        public void Configure(EntityTypeBuilder<ManualPaymentRequest> builder)
        {
            builder.ToTable("ManualPaymentRequests");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(m => m.ContentId)
                .IsRequired();

            builder.Property(m => m.TransferMethod)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(m => m.ReferenceNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.ScreenshotUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(m => m.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(ManualPaymentStatus.Pending);

            builder.Property(m => m.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(m => m.ReviewedAt);

            builder.Property(m => m.ReviewedBy)
                .HasMaxLength(450);

            builder.Property(m => m.RejectionReason)
                .HasMaxLength(500);

            builder.HasOne(m => m.Content)
                .WithMany()
                .HasForeignKey(m => m.ContentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => m.UserId);
            builder.HasIndex(m => m.Status);
            builder.HasIndex(m => m.CreatedAt);
        }
    }
}
