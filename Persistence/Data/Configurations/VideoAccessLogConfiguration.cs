using Domain.Entities.VideoEntities;
using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations;

public class VideoAccessLogConfiguration : IEntityTypeConfiguration<VideoAccessLog>
{
    public void Configure(EntityTypeBuilder<VideoAccessLog> builder)
    {
        builder.ToTable("VideoAccessLogs");

        builder.HasKey(val => val.Id);

        builder.Property(val => val.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(val => val.VideoType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(val => val.IpAddress)
            .IsRequired()
            .HasMaxLength(45); 

        builder.Property(val => val.AccessedAt)
            .IsRequired();

        builder.HasOne(val => val.Course)
            .WithMany()
            .HasForeignKey(val => val.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(val => val.User)
            .WithMany()
            .HasForeignKey(val => val.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(val => val.UserId);
        builder.HasIndex(val => val.CourseId);
        builder.HasIndex(val => val.VideoId);
        builder.HasIndex(val => val.AccessedAt);
        builder.HasIndex(val => val.IpAddress);
    }
}
