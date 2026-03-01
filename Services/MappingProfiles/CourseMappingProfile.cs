using AutoMapper;
using Domain.Entities.CourseEntities;
using Shared.CourseModels;
using System.Text.Json;

namespace Services.MappingProfiles
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            // E-wallet method mappings
            CreateMap<CourseEWalletMethod, EWalletMethodDTO>();

            // Course mappings
            CreateMap<Course, CourseResponseDTO>()
                .ForMember(dest => dest.TotalVideos, opt => opt.MapFrom(src => 0)) // Calculated separately
                .ForMember(dest => dest.TotalEnrollments, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 0.0))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsEnrolled, opt => opt.MapFrom(src => false));

            CreateMap<Course, CourseDetailDTO>()
                .ForMember(dest => dest.Requirements, opt => opt.MapFrom(src => ParseJsonArray(src.Requirements)))
                .ForMember(dest => dest.LearningObjectives, opt => opt.MapFrom(src => ParseJsonArray(src.LearningObjectives)))
                .ForMember(dest => dest.TotalVideos, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalLiveSessions, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalEnrollments, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 0.0))
                .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsEnrolled, opt => opt.MapFrom(src => false));

            // Enrollment mappings
            CreateMap<Enrollment, EnrollmentResponseDTO>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => string.Empty)) // Set from Course
                .ForMember(dest => dest.EnrollmentSource, opt => opt.MapFrom(src => src.Source.ToString()));

            // Review mappings
            CreateMap<CourseReview, CourseReviewResponseDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => string.Empty)); // Set separately
        }

        private static string[]? ParseJsonArray(string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                return JsonSerializer.Deserialize<string[]>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}
