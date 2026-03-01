using AutoMapper;
using Domain.Entities.VideoEntities;
using Shared.Helpers;
using Shared.VideoModels;

namespace Services.MappingProfiles;

public class VideoMappingProfile : Profile
{
    public VideoMappingProfile()
    {
        // CourseVideo mappings
        CreateMap<CreateCourseVideoDTO, CourseVideo>();
        CreateMap<UpdateCourseVideoDTO, CourseVideo>();
        CreateMap<CourseVideo, CourseVideoResponseDTO>();

        // LiveSession mappings
        CreateMap<CreateLiveSessionDTO, LiveSession>();
        CreateMap<UpdateLiveSessionDTO, LiveSession>();
        CreateMap<LiveSession, LiveSessionResponseDTO>()
            .ForMember(dest => dest.IsLive, opt => opt.MapFrom(src => 
                src.IsActive && 
                EgyptDateTime.Now >= src.ScheduledStart && 
                EgyptDateTime.Now <= src.ScheduledEnd))
            .ForMember(dest => dest.RecordingLink, opt => opt.MapFrom(src =>
                src.IsRecordedAvailable ? src.RecordingVideoId : null))
            .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.RoomName))
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src.Provider ?? "JITSI"));

        // VideoProgress mappings
        CreateMap<VideoProgress, VideoProgressResponseDTO>();
    }
}
