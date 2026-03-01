using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class CourseReviewConfiguration : IEntityTypeConfiguration<CourseReview>
    {
        public void Configure(EntityTypeBuilder<CourseReview> builder)
        {
            builder.ToTable("CourseReviews");

            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.CourseId)
                .IsRequired();

            builder.Property(r => r.StudentId)
                .IsRequired()
                .HasMaxLength(450); 

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.ReviewText)
                .HasMaxLength(2000);

            builder.Property(r => r.IsApproved)
                .HasDefaultValue(true);

            builder.Property(r => r.IsHidden)
                .HasDefaultValue(false);

            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(r => r.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Foreign Keys
            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => r.CourseId)
                .HasDatabaseName("IX_CourseReviews_CourseId");

            builder.HasIndex(r => r.StudentId)
                .HasDatabaseName("IX_CourseReviews_StudentId");

            builder.HasIndex(r => r.Rating)
                .HasDatabaseName("IX_CourseReviews_Rating");

            builder.HasIndex(r => r.IsApproved)
                .HasDatabaseName("IX_CourseReviews_IsApproved");

            builder.HasIndex(r => r.CreatedAt)
                .HasDatabaseName("IX_CourseReviews_CreatedAt");
        }
    }
}
