using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CourseEntities;
using Domain.Entities.VideoEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.Helpers;
using Shared.VideoModels;

namespace Services;

public class VideoService : IVideoService
{
    private readonly IUnitOFWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICourseAccessService _courseAccessService;

    public VideoService(IUnitOFWork unitOfWork, IMapper mapper, ICourseAccessService courseAccessService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _courseAccessService = courseAccessService;
    }

    public async Task<CourseVideoResponseDTO> CreateCourseVideoAsync(Guid courseId, CreateCourseVideoDTO dto)
    {
        // Validate course exists
        var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
        var course = await courseRepo.GetAsync(courseId);
        
        if (course == null)
        {
            throw new ProductNotFoundException(courseId.ToString());
        }

        var video = _mapper.Map<CourseVideo>(dto);
        video.CourseId = courseId;
        video.CreatedAt = EgyptDateTime.Now;

        var videoRepo = _unitOfWork.GetRepository<CourseVideo, Guid>();
        await videoRepo.AddAsync(video);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseVideoResponseDTO>(video);
    }

    public async Task<CourseVideoResponseDTO> UpdateCourseVideoAsync(Guid videoId, UpdateCourseVideoDTO dto)
    {
        var videoRepo = _unitOfWork.GetRepository<CourseVideo, Guid>();
        var video = await videoRepo.GetAsync(videoId);

        if (video == null)
        {
            throw new VideoNotFoundException(videoId);
        }

        _mapper.Map(dto, video);
        videoRepo.Update(video);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CourseVideoResponseDTO>(video);
    }

    public async Task DeleteCourseVideoAsync(Guid videoId)
    {
        var videoRepo = _unitOfWork.GetRepository<CourseVideo, Guid>();
        var video = await videoRepo.GetAsync(videoId);

        if (video == null)
        {
            throw new VideoNotFoundException(videoId);
        }

        videoRepo.Delete(video);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<CourseVideoResponseDTO>> GetCourseVideosAsync(Guid courseId)
    {
        var videoRepo = _unitOfWork.GetRepository<CourseVideo, Guid>();
        var allVideos = await videoRepo.GetAllAsync(trackChanges: false);
        
        var videos = allVideos.Where(v => v.CourseId == courseId).OrderBy(v => v.OrderIndex);
        
        return _mapper.Map<IEnumerable<CourseVideoResponseDTO>>(videos);
    }

    public async Task<VideoStreamResponseDTO> GetVideoStreamAsync(Guid courseId, Guid videoId, string userId, string ipAddress)
    {
        var videoRepo = _unitOfWork.GetRepository<CourseVideo, Guid>();
        var video = await videoRepo.GetAsync(videoId);

        if (video == null)
        {
            throw new VideoNotFoundException(videoId);
        }

        // Verify video belongs to the specified course (prevent enumeration)
        if (video.CourseId != courseId)
        {
            throw new VideoNotFoundException(videoId);
        }

        // If not preview, validate access
        if (!video.IsPreview)
        {
            await _courseAccessService.ValidateCourseAccessAsync(courseId, userId);
        }

        // Log access
        await LogVideoAccessAsync(userId, courseId, videoId, "CourseVideo", ipAddress);

        // Generate embed URL
        var embedUrl = GenerateYouTubeEmbedUrl(video.VideoId);

        return new VideoStreamResponseDTO
        {
            Title = video.Title,
            Description = video.Description,
            Duration = video.Duration,
            EmbedUrl = embedUrl,
            IsPreview = video.IsPreview
        };
    }

    public async Task<VideoProgressResponseDTO> UpdateVideoProgressAsync(Guid videoId, string userId, UpdateVideoProgressDTO dto)
    {
        var videoRepo = _unitOfWork.GetRepository<CourseVideo, Guid>();
        var video = await videoRepo.GetAsync(videoId);

        if (video == null)
        {
            throw new VideoNotFoundException(videoId);
        }

        var progressRepo = _unitOfWork.GetRepository<VideoProgress, Guid>();
        var allProgress = await progressRepo.GetAllAsync(trackChanges: true);
        var existingProgress = allProgress.FirstOrDefault(p => p.VideoId == videoId && p.UserId == userId);

        if (existingProgress == null)
        {
            // Create new progress
            var newProgress = new VideoProgress
            {
                UserId = userId,
                VideoId = videoId,
                WatchedSeconds = dto.WatchedSeconds,
                IsCompleted = CalculateCompletion(dto.WatchedSeconds, video.Duration),
                LastUpdated = EgyptDateTime.Now
            };

            await progressRepo.AddAsync(newProgress);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VideoProgressResponseDTO>(newProgress);
        }
        else
        {
            // Update existing progress
            existingProgress.WatchedSeconds = dto.WatchedSeconds;
            existingProgress.IsCompleted = CalculateCompletion(dto.WatchedSeconds, video.Duration);
            existingProgress.LastUpdated = EgyptDateTime.Now;

            progressRepo.Update(existingProgress);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VideoProgressResponseDTO>(existingProgress);
        }
    }

    public async Task<VideoProgressResponseDTO?> GetVideoProgressAsync(Guid videoId, string userId)
    {
        var progressRepo = _unitOfWork.GetRepository<VideoProgress, Guid>();
        var allProgress = await progressRepo.GetAllAsync(trackChanges: false);
        var progress = allProgress.FirstOrDefault(p => p.VideoId == videoId && p.UserId == userId);

        return progress == null ? null : _mapper.Map<VideoProgressResponseDTO>(progress);
    }

    public async Task<IEnumerable<VideoProgressResponseDTO>> GetCourseProgressAsync(Guid courseId, string userId)
    {
        var videoRepo = _unitOfWork.GetRepository<CourseVideo, Guid>();
        var allVideos = await videoRepo.GetAllAsync(trackChanges: false);
        var courseVideos = allVideos.Where(v => v.CourseId == courseId);
        var videoIds = courseVideos.Select(v => v.Id).ToList();

        var progressRepo = _unitOfWork.GetRepository<VideoProgress, Guid>();
        var allProgress = await progressRepo.GetAllAsync(trackChanges: false);
        var courseProgress = allProgress.Where(p => p.UserId == userId && videoIds.Contains(p.VideoId));

        return _mapper.Map<IEnumerable<VideoProgressResponseDTO>>(courseProgress);
    }

    private async Task LogVideoAccessAsync(string userId, Guid courseId, Guid videoId, string videoType, string ipAddress)
    {
        var log = new VideoAccessLog
        {
            UserId = userId,
            CourseId = courseId,
            VideoId = videoId,
            VideoType = videoType,
            AccessedAt = EgyptDateTime.Now,
            IpAddress = ipAddress
        };

        var logRepo = _unitOfWork.GetRepository<VideoAccessLog, Guid>();
        await logRepo.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }

    private bool CalculateCompletion(int watchedSeconds, int totalDuration)
    {
        if (totalDuration == 0) return false;

        var percentage = (double)watchedSeconds / totalDuration;
        return percentage >= 0.90; // 90% completion threshold
    }

    private string GenerateYouTubeEmbedUrl(string videoId)
    {
        return $"https://www.youtube.com/embed/{videoId}?rel=0&modestbranding=1";
    }
}
