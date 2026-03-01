using Domain.Entities.VideoEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations;

public class VideoProgressConfiguration : IEntityTypeConfiguration<VideoProgress>
{
    public void Configure(EntityTypeBuilder<VideoProgress> builder)
    {
        builder.ToTable("VideoProgress");

        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(vp => vp.WatchedSeconds)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(vp => vp.IsCompleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(vp => vp.LastUpdated)
            .IsRequired();

        builder.HasOne(vp => vp.Video)
            .WithMany(v => v.VideoProgresses)
            .HasForeignKey(vp => vp.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vp => vp.User)
            .WithMany()
            .HasForeignKey(vp => vp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(vp => vp.UserId);
        builder.HasIndex(vp => vp.VideoId);
        builder.HasIndex(vp => new { vp.UserId, vp.VideoId })
            .IsUnique();
    }
}
