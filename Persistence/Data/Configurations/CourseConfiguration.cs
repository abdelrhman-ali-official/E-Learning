using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");

            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(c => c.ThumbnailUrl)
                .HasMaxLength(500);

            builder.Property(c => c.InstructorId)
                .IsRequired()
                .HasMaxLength(450); // ASP.NET Identity default FK length

            builder.Property(c => c.InstructorName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Price)
                .HasPrecision(18, 2);

            builder.Property(c => c.Category)
                .HasMaxLength(100);

            builder.Property(c => c.Level)
                .HasMaxLength(50);

            builder.Property(c => c.Requirements)
                .HasMaxLength(2000); // JSON string

            builder.Property(c => c.LearningObjectives)
                .HasMaxLength(2000); // JSON string

            // Defaults
            builder.Property(c => c.IsFree)
                .HasDefaultValue(false);

            builder.Property(c => c.IsPublished)
                .HasDefaultValue(false);

            builder.Property(c => c.IsFeatured)
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(c => c.EstimatedDurationMinutes)
                .HasDefaultValue(0);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(c => c.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Courses_Slug");

            builder.HasIndex(c => c.IsPublished)
                .HasDatabaseName("IX_Courses_IsPublished");

            builder.HasIndex(c => c.IsFeatured)
                .HasDatabaseName("IX_Courses_IsFeatured");

            builder.HasIndex(c => c.IsDeleted)
                .HasDatabaseName("IX_Courses_IsDeleted");

            builder.HasIndex(c => c.Category)
                .HasDatabaseName("IX_Courses_Category");

            builder.HasIndex(c => c.InstructorId)
                .HasDatabaseName("IX_Courses_InstructorId");

            builder.HasIndex(c => c.CreatedAt)
                .HasDatabaseName("IX_Courses_CreatedAt");

            // Note: Navigation properties will be configured when we update VideoEntities
            // to reference Course instead of Product
        }
    }
}
