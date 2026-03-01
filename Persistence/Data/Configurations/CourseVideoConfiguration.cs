using Domain.Entities.VideoEntities;
using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations;

public class CourseVideoConfiguration : IEntityTypeConfiguration<CourseVideo>
{
    public void Configure(EntityTypeBuilder<CourseVideo> builder)
    {
        builder.ToTable("CourseVideos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(v => v.Description)
            .HasMaxLength(2000);

        builder.Property(v => v.VideoId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Duration)
            .IsRequired();

        builder.Property(v => v.OrderIndex)
            .IsRequired();

        builder.Property(v => v.IsPreview)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.HasOne(v => v.Course)
            .WithMany()
            .HasForeignKey(v => v.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.VideoProgresses)
            .WithOne(vp => vp.Video)
            .HasForeignKey(vp => vp.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.AccessLogs)
            .WithOne()
            .HasForeignKey(al => al.VideoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(v => v.CourseId);
        builder.HasIndex(v => v.OrderIndex);
        builder.HasIndex(v => v.IsPreview);
    }
}
