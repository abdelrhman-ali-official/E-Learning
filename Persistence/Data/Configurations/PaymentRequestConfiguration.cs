using Domain.Entities.SubscriptionEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class PaymentRequestConfiguration : IEntityTypeConfiguration<PaymentRequest>
    {
        public void Configure(EntityTypeBuilder<PaymentRequest> builder)
        {
            builder.ToTable("PaymentRequests");

            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.StudentSubscriptionId)
                .IsRequired();

            builder.Property(pr => pr.StudentId)
                .IsRequired();

            builder.Property(pr => pr.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(pr => pr.PaymentMethod)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(pr => pr.TransactionReference)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pr => pr.PaymentProofUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pr => pr.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(pr => pr.AdminNotes)
                .HasMaxLength(500);

            builder.Property(pr => pr.RequestedAt)
                .IsRequired();

            builder.HasOne(pr => pr.StudentSubscription)
                .WithMany(s => s.PaymentRequests)
                .HasForeignKey(pr => pr.StudentSubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pr => pr.StudentId);
            builder.HasIndex(pr => pr.Status);
            builder.HasIndex(pr => pr.StudentSubscriptionId);
            builder.HasIndex(pr => new { pr.StudentId, pr.Status });
        }
    }
}
