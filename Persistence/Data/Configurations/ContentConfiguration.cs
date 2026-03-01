using Domain.Entities.ContentEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.ToTable("Contents");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.ThumbnailUrl)
                .HasMaxLength(500);

            builder.Property(c => c.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.AccessDurationWeeks)
                .IsRequired();

            builder.Property(c => c.YoutubeVideoId)
                .HasMaxLength(100);

            builder.Property(c => c.VideoObjectKey)
                .HasMaxLength(500);

            builder.Property(c => c.LiveStreamUrl)
                .HasMaxLength(500);

            builder.Property(c => c.ExternalVideoUrl)
                .HasMaxLength(1000);

            builder.Property(c => c.IsLiveActive)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsVisible)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.InstructorId)
                .IsRequired()
                .HasMaxLength(450);

            builder.HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationships
            builder.HasMany(c => c.Purchases)
                .WithOne(p => p.Content)
                .HasForeignKey(p => p.ContentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.WatchProgress)
                .WithOne(wp => wp.Content)
                .HasForeignKey(wp => wp.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
