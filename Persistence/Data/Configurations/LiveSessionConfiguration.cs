using Domain.Entities.VideoEntities;
using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations;

public class LiveSessionConfiguration : IEntityTypeConfiguration<LiveSession>
{
    public void Configure(EntityTypeBuilder<LiveSession> builder)
    {
        builder.ToTable("LiveSessions");

        builder.HasKey(ls => ls.Id);

        builder.Property(ls => ls.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ls => ls.Description)
            .HasMaxLength(2000);

        builder.Property(ls => ls.MeetingLink)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(ls => ls.InstructorId)
            .IsRequired()
            .HasMaxLength(450); 

        builder.Property(ls => ls.YouTubeLiveVideoId)
            .HasMaxLength(50);

        builder.Property(ls => ls.RoomName)
            .HasMaxLength(300);

        builder.Property(ls => ls.Provider)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("JITSI");

        builder.Property(ls => ls.RecordingVideoId)
            .HasMaxLength(500);

        builder.Property(ls => ls.ScheduledStart)
            .IsRequired();

        builder.Property(ls => ls.ScheduledEnd)
            .IsRequired();

        builder.Property(ls => ls.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ls => ls.IsRecordedAvailable)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ls => ls.CreatedAt)
            .IsRequired();

        builder.Property(ls => ls.UpdatedAt)
            .IsRequired();

        builder.HasOne(ls => ls.Course)
            .WithMany()
            .HasForeignKey(ls => ls.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ls => ls.AccessLogs)
            .WithOne()
            .HasForeignKey(al => al.VideoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ls => ls.CourseId);
        builder.HasIndex(ls => ls.InstructorId);
        builder.HasIndex(ls => ls.IsActive);
        builder.HasIndex(ls => ls.ScheduledStart);
        builder.HasIndex(ls => ls.ScheduledEnd);
    }
}
