using Domain.Entities.CourseEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    public class CourseEWalletMethodConfiguration : IEntityTypeConfiguration<CourseEWalletMethod>
    {
        public void Configure(EntityTypeBuilder<CourseEWalletMethod> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.InstructorId).IsRequired().HasMaxLength(450);
            builder.Property(e => e.MethodName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.WalletNumber).IsRequired().HasMaxLength(50);
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.CreatedAt).IsRequired();

            builder.HasOne(e => e.Course)
                   .WithMany()
                   .HasForeignKey(e => e.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
