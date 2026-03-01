using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IServiceManager
    {
        public IProductService ProductService { get; }
        public IAuthenticationService AuthenticationService { get; }
        public IContentService ContentService { get; }
        public IPurchaseService PurchaseService { get; }
        public IManualPaymentService ManualPaymentService { get; }
        public IStorageService StorageService { get; }
        public IPackageService PackageService { get; }
        public ISubscriptionService SubscriptionService { get; }
        public IPaymentRequestService PaymentRequestService { get; }
        public ICouponService CouponService { get; }
        public ISubscriptionAnalyticsService SubscriptionAnalyticsService { get; }
        public IVideoService VideoService { get; }
        public ILiveSessionService LiveSessionService { get; }
        public ICourseAccessService CourseAccessService { get; }
        public ICourseService CourseService { get; }
        public IEnrollmentService EnrollmentService { get; }
        public ICoursePaymentService CoursePaymentService { get; }
        public IInstructorAnalyticsService InstructorAnalyticsService { get; }
    }
}