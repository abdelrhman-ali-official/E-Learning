using Domain.Contracts;
using Domain.Entities.SecurityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Abstractions;
using Shared.JaaS;
using Shared.SecurityModels;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace Services
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<IProductService> _productService;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IContentService> _contentService;
        private readonly Lazy<IPurchaseService> _purchaseService;
        private readonly Lazy<IManualPaymentService> _manualPaymentService;
        private readonly Lazy<IStorageService> _storageService;
        private readonly Lazy<IPackageService> _packageService;
        private readonly Lazy<ISubscriptionService> _subscriptionService;
        private readonly Lazy<IPaymentRequestService> _paymentRequestService;
        private readonly Lazy<ICouponService> _couponService;
        private readonly Lazy<ISubscriptionAnalyticsService> _subscriptionAnalyticsService;
        private readonly Lazy<ICourseAccessService> _courseAccessService;
        private readonly Lazy<IVideoService> _videoService;
        private readonly Lazy<ILiveSessionService> _liveSessionService;
        private readonly Lazy<ICourseService> _courseService;
        private readonly Lazy<IEnrollmentService> _enrollmentService;
        private readonly Lazy<ICoursePaymentService> _coursePaymentService;
        private readonly Lazy<IInstructorAnalyticsService> _instructorAnalyticsService;
        private readonly AutoMapper.IMapper _mapper;

        public ServiceManager(
            IUnitOFWork unitOfWork,
            AutoMapper.IMapper mapper,
            UserManager<User> userManager,
            IOptions<JwtOptions> jwtOptions,
            IOptions<DomainSettings> domainSettings,
            IOptions<JaaSOptions> jaasOptions,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            
            _productService = new Lazy<IProductService>(() => new ProductService(unitOfWork, mapper));

            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                userManager,
                jwtOptions,
                domainSettings,
                mapper,
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>()
            ));

            _contentService = new Lazy<IContentService>(() => new ContentService(unitOfWork, mapper));
            
            _purchaseService = new Lazy<IPurchaseService>(() => new PurchaseService(unitOfWork, mapper));
            
            _manualPaymentService = new Lazy<IManualPaymentService>(() => new ManualPaymentService(unitOfWork, mapper));
            
            _storageService = new Lazy<IStorageService>(() => new CloudflareR2StorageService(configuration));
            
            _packageService = new Lazy<IPackageService>(() => new PackageService(unitOfWork, mapper));
            
            _couponService = new Lazy<ICouponService>(() => new CouponService(unitOfWork, mapper));
            
            _subscriptionService = new Lazy<ISubscriptionService>(() => new SubscriptionService(unitOfWork, mapper, _couponService.Value));
            
            _paymentRequestService = new Lazy<IPaymentRequestService>(() => new PaymentRequestService(unitOfWork, mapper, _storageService.Value, _couponService.Value));
            
            _subscriptionAnalyticsService = new Lazy<ISubscriptionAnalyticsService>(() => new SubscriptionAnalyticsService(unitOfWork, mapper));
            
            _courseAccessService = new Lazy<ICourseAccessService>(() => new CourseAccessService(unitOfWork));
            
            var jaasTokenService = new JaaSTokenService(jaasOptions);
            _videoService = new Lazy<IVideoService>(() => new VideoService(unitOfWork, mapper, _courseAccessService.Value));
            
            _liveSessionService = new Lazy<ILiveSessionService>(() => new LiveSessionService(unitOfWork, mapper, _courseAccessService.Value, jaasTokenService));
            
            _courseService = new Lazy<ICourseService>(() => new CourseService(unitOfWork, mapper));
            
            _enrollmentService = new Lazy<IEnrollmentService>(() => new EnrollmentService(unitOfWork, mapper));

            _coursePaymentService = new Lazy<ICoursePaymentService>(() => new CoursePaymentService(unitOfWork, mapper, userManager));

            _instructorAnalyticsService = new Lazy<IInstructorAnalyticsService>(() => new InstructorAnalyticsService(unitOfWork));
        }

        public IProductService ProductService => _productService.Value;
        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IContentService ContentService => _contentService.Value;
        public IPurchaseService PurchaseService => _purchaseService.Value;
        public IManualPaymentService ManualPaymentService => _manualPaymentService.Value;
        public ICourseAccessService CourseAccessService => _courseAccessService.Value;
        public IVideoService VideoService => _videoService.Value;
        public ILiveSessionService LiveSessionService => _liveSessionService.Value;
        public IStorageService StorageService => _storageService.Value;
        public IPackageService PackageService => _packageService.Value;
        public ICourseService CourseService => _courseService.Value;
        public IEnrollmentService EnrollmentService => _enrollmentService.Value;
        public ICoursePaymentService CoursePaymentService => _coursePaymentService.Value;
        public ISubscriptionService SubscriptionService => _subscriptionService.Value;
        public IPaymentRequestService PaymentRequestService => _paymentRequestService.Value;
        public ICouponService CouponService => _couponService.Value;
        public ISubscriptionAnalyticsService SubscriptionAnalyticsService => _subscriptionAnalyticsService.Value;
        public IInstructorAnalyticsService InstructorAnalyticsService => _instructorAnalyticsService.Value;
    }
}