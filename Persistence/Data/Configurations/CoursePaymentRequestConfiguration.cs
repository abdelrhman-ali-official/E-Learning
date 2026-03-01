using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class CoursePaymentRequestConfiguration : IEntityTypeConfiguration<CoursePaymentRequest>
    {
        public void Configure(EntityTypeBuilder<CoursePaymentRequest> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.StudentId).IsRequired().HasMaxLength(450);
            builder.Property(r => r.StudentWalletNumber).IsRequired().HasMaxLength(50);
            builder.Property(r => r.ScreenshotUrl).IsRequired().HasMaxLength(1000);
            builder.Property(r => r.Amount).HasColumnType("decimal(18,2)");
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.ReviewedBy).HasMaxLength(450);
            builder.Property(r => r.RejectionReason).HasMaxLength(500);

            builder.HasOne(r => r.Course)
                   .WithMany()
                   .HasForeignKey(r => r.CourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.EWalletMethod)
                   .WithMany()
                   .HasForeignKey(r => r.EWalletMethodId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
