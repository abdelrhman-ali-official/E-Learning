using AutoMapper;
using Domain.Entities.SubscriptionEntities;
using Shared.SubscriptionModels;

namespace Services.MappingProfiles
{
    public class SubscriptionMappingProfile : Profile
    {
        public SubscriptionMappingProfile()
        {
            // Package mappings
            CreateMap<CreatePackageDTO, Package>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));

            CreateMap<UpdatePackageDTO, Package>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));

            CreateMap<Package, PackageResponseDTO>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features));

            CreateMap<PackageFeatureDTO, PackageFeature>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PackageId, opt => opt.Ignore())
                .ForMember(dest => dest.Package, opt => opt.Ignore());

            CreateMap<PackageFeature, PackageFeatureDTO>();

            // Subscription mappings
            CreateMap<StudentSubscription, SubscriptionResponseDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.DisplayName : string.Empty))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student != null ? src.Student.Email : string.Empty))
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package != null ? src.Package.Name : string.Empty))
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.DiscountCoupon != null ? src.DiscountCoupon.Code : null));

            // Payment Request mappings
            CreateMap<PaymentRequest, PaymentResponseDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.Ignore())
                .ForMember(dest => dest.ReviewedByEmail, opt => opt.Ignore());

            // Coupon mappings
            CreateMap<CreateCouponDTO, DiscountCoupon>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UsedCount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Subscriptions, opt => opt.Ignore());

            CreateMap<DiscountCoupon, CouponResponseDTO>();
        }
    }
}
