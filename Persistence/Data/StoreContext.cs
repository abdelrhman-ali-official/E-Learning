global using Microsoft.EntityFrameworkCore;
using Domain.Entities.ProductEntities;
using Domain.Entities.SecurityEntities;
using Domain.Entities.ContentEntities;
using Domain.Entities.SubscriptionEntities;
using Domain.Entities.VideoEntities;
using Domain.Entities.CourseEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data
{
    public class StoreContext : IdentityDbContext<User>
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ManualPaymentRequest> ManualPaymentRequests { get; set; }
        public DbSet<WatchProgress> WatchProgress { get; set; }
        
        // Subscription entities
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageFeature> PackageFeatures { get; set; }
        public DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<DiscountCoupon> DiscountCoupons { get; set; }
        
        // Video entities
        public DbSet<CourseVideo> CourseVideos { get; set; }
        public DbSet<LiveSession> LiveSessions { get; set; }
        public DbSet<VideoProgress> VideoProgress { get; set; }
        public DbSet<VideoAccessLog> VideoAccessLogs { get; set; }
        
        // Course entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseReview> CourseReviews { get; set; }
        public DbSet<CourseEWalletMethod> CourseEWalletMethods { get; set; }
        public DbSet<CoursePaymentRequest> CoursePaymentRequests { get; set; }
    }
}

