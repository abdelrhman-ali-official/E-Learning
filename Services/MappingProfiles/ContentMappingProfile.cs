using AutoMapper;
using Domain.Entities.ContentEntities;
using Shared.ContentModels;

namespace Services.MappingProfiles
{
    public class ContentMappingProfile : Profile
    {
        public ContentMappingProfile()
        {
            // Content mappings
            CreateMap<Content, ContentResultDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            // Purchase mappings
            CreateMap<Purchase, PurchaseResultDTO>()
                .ForMember(dest => dest.ContentTitle, opt => opt.MapFrom(src => src.Content.Title));

            // Manual Payment Request mappings
            CreateMap<ManualPaymentRequest, ManualPaymentRequestResultDTO>()
                .ForMember(dest => dest.ContentTitle, opt => opt.MapFrom(src => src.Content.Title))
                .ForMember(dest => dest.TransferMethod, opt => opt.MapFrom(src => src.TransferMethod.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}
