using Domain.Entities.ContentEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class WatchProgressConfiguration : IEntityTypeConfiguration<WatchProgress>
    {
        public void Configure(EntityTypeBuilder<WatchProgress> builder)
        {
            builder.ToTable("WatchProgress");

            builder.HasKey(wp => wp.Id);

            builder.Property(wp => wp.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(wp => wp.LastPositionSeconds)
                .IsRequired();

            builder.Property(wp => wp.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(wp => wp.LastUpdated)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(wp => wp.User)
                .WithMany()
                .HasForeignKey(wp => wp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wp => wp.Content)
                .WithMany(c => c.WatchProgress)
                .HasForeignKey(wp => wp.ContentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(wp => new { wp.UserId, wp.ContentId })
                .IsUnique()
                .HasDatabaseName("IX_WatchProgress_UserId_ContentId");
        }
    }
}
