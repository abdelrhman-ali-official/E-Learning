using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.CourseId)
                .IsRequired();

            builder.Property(e => e.StudentId)
                .IsRequired()
                .HasMaxLength(450); 

            builder.Property(e => e.Source)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.ProgressPercentage)
                .HasDefaultValue(0);

            builder.Property(e => e.CompletedVideos)
                .HasDefaultValue(0);

            builder.Property(e => e.TotalVideos)
                .HasDefaultValue(0);

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true);

            builder.Property(e => e.IsCertificateIssued)
                .HasDefaultValue(false);

            builder.Property(e => e.EnrolledAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.LastAccessedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.CourseId)
                .HasDatabaseName("IX_Enrollments_CourseId");

            builder.HasIndex(e => e.StudentId)
                .HasDatabaseName("IX_Enrollments_StudentId");

            builder.HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique()
                .HasDatabaseName("IX_Enrollments_StudentId_CourseId");

            builder.HasIndex(e => e.Source)
                .HasDatabaseName("IX_Enrollments_Source");

            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_Enrollments_IsActive");

            builder.HasIndex(e => e.LastAccessedAt)
                .HasDatabaseName("IX_Enrollments_LastAccessedAt");
        }
    }
}
