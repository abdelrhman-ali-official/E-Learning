global using Domain.Contracts;
global using Services.Abstractions;
global using AutoMapper;
using Domain.Entities.ContentEntities;
using Domain.Entities.CourseEntities;
using Domain.Exceptions;
using Services.Helpers;
using Shared;
using Shared.ContentModels;
using Shared.Helpers;

namespace Services
{
    public class ContentService(IUnitOFWork unitOfWork, IMapper mapper) : IContentService
    {

        public async Task<ContentResultDTO> CreateCourseContentAsync(Guid courseId, string instructorId, CreateContentDTO dto)
        {
            // Validate course exists
            var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new ProductNotFoundException(courseId.ToString());

            // Resolve MediaLink → YoutubeVideoId or ExternalVideoUrl
            string? youtubeVideoId = null;
            string? externalVideoUrl = null;

            if (!string.IsNullOrWhiteSpace(dto.MediaLink))
            {
                if (!UrlHelper.IsValidUrl(dto.MediaLink))
                    throw new ValidationException(new[] { "MediaLink must be a valid URL" });

                if (UrlHelper.IsYouTubeUrl(dto.MediaLink))
                    youtubeVideoId = UrlHelper.ExtractYouTubeVideoId(dto.MediaLink) ?? dto.MediaLink;
                else if (UrlHelper.IsGoogleDriveUrl(dto.MediaLink))
                    externalVideoUrl = UrlHelper.ConvertGoogleDriveToPreview(dto.MediaLink);
                else
                    externalVideoUrl = dto.MediaLink;
            }

            var content = new Content
            {
                CourseId    = courseId,
                InstructorId = instructorId,
                Title       = dto.Title,
                Description = dto.Description ?? string.Empty,
                ThumbnailUrl = dto.ThumbnailUrl,
                Type        = (ContentType)dto.Type,
                YoutubeVideoId = youtubeVideoId,
                ExternalVideoUrl = externalVideoUrl,
                IsVisible   = dto.IsVisible,
                IsDownloadable = dto.IsDownloadable,
                CreatedAt   = EgyptDateTime.Now,
                Price = 0,
                AccessDurationWeeks = 0,
            };

            await unitOfWork.GetRepository<Content, int>().AddAsync(content);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ContentResultDTO>(content);
        }

        public async Task<ContentResultDTO> UpdateCourseContentAsync(int id, string instructorId, UpdateContentDTO dto)
        {
            var content = await unitOfWork.GetRepository<Content, int>().GetAsync(id)
                ?? throw new ContentNotFoundException(id);

            if (content.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to edit this content");

            if (dto.Title != null) content.Title = dto.Title;
            if (dto.Description != null) content.Description = dto.Description;
            if (dto.ThumbnailUrl != null) content.ThumbnailUrl = dto.ThumbnailUrl;
            if (dto.IsVisible.HasValue) content.IsVisible = dto.IsVisible.Value;
            if (dto.IsDownloadable.HasValue) content.IsDownloadable = dto.IsDownloadable.Value;

            if (dto.MediaLink != null)
            {
                if (!UrlHelper.IsValidUrl(dto.MediaLink))
                    throw new ValidationException(new[] { "MediaLink must be a valid URL" });

                if (UrlHelper.IsYouTubeUrl(dto.MediaLink))
                {
                    content.YoutubeVideoId = UrlHelper.ExtractYouTubeVideoId(dto.MediaLink) ?? dto.MediaLink;
                    content.ExternalVideoUrl = null;
                }
                else if (UrlHelper.IsGoogleDriveUrl(dto.MediaLink))
                {
                    content.ExternalVideoUrl = UrlHelper.ConvertGoogleDriveToPreview(dto.MediaLink);
                    content.YoutubeVideoId = null;
                }
                else
                {
                    content.ExternalVideoUrl = dto.MediaLink;
                    content.YoutubeVideoId = null;
                }
            }

            unitOfWork.GetRepository<Content, int>().Update(content);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ContentResultDTO>(content);
        }

        public async Task DeleteCourseContentAsync(int id, string instructorId)
        {
            var content = await unitOfWork.GetRepository<Content, int>().GetAsync(id)
                ?? throw new ContentNotFoundException(id);

            if (content.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to delete this content");

            unitOfWork.GetRepository<Content, int>().Delete(content);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContentResultDTO>> GetCourseContentsAsync(Guid courseId)
        {
            var all = await unitOfWork.GetRepository<Content, int>().GetAllAsync(trackChanges: false);
            var courseContents = all
                .Where(c => c.CourseId == courseId)
                .OrderByDescending(c => c.CreatedAt);

            return mapper.Map<IEnumerable<ContentResultDTO>>(courseContents);
        }

        public async Task<ContentResultDTO> GetCourseContentByIdAsync(Guid courseId, int id)
        {
            var content = await unitOfWork.GetRepository<Content, int>().GetAsync(id)
                ?? throw new ContentNotFoundException(id);

            if (content.CourseId != courseId)
                throw new ContentNotFoundException(id);

            return mapper.Map<ContentResultDTO>(content);
        }


        public async Task<IEnumerable<PublicCourseContentDTO>> GetPublicCourseContentsAsync(Guid courseId)
        {
            var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new ProductNotFoundException(courseId.ToString());

            if (course.IsDeleted || !course.IsPublished)
                throw new ProductNotFoundException(courseId.ToString());

            var all = await unitOfWork.GetRepository<Content, int>().GetAllAsync(trackChanges: false);
            return all
                .Where(c => c.CourseId == courseId && c.IsVisible)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new PublicCourseContentDTO
                {
                    Id           = c.Id,
                    Title        = c.Title,
                    Type         = c.Type.ToString(),
                    Description  = string.IsNullOrEmpty(c.Description) ? null : c.Description,
                    ThumbnailUrl = c.ThumbnailUrl,
                    IsDownloadable = c.IsDownloadable
                })
                .ToList();
        }


        public async Task<PaginatedResult<ContentResultDTO>> GetAllContentsAsync(int pageIndex = 1, int pageSize = 10)
        {
            var all = await unitOfWork.GetRepository<Content, int>().GetAllAsync();
            var totalCount = all.Count();
            var paged = all
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return new PaginatedResult<ContentResultDTO>(
                pageIndex,
                paged.Count(),
                totalCount,
                mapper.Map<IEnumerable<ContentResultDTO>>(paged));
        }

        public async Task DeleteContentAsync(int id)
        {
            var content = await unitOfWork.GetRepository<Content, int>().GetAsync(id)
                ?? throw new ContentNotFoundException(id);

            unitOfWork.GetRepository<Content, int>().Delete(content);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<ContentResultDTO> ToggleVisibilityAsync(int id)
        {
            var content = await unitOfWork.GetRepository<Content, int>().GetAsync(id)
                ?? throw new ContentNotFoundException(id);

            content.IsVisible = !content.IsVisible;
            unitOfWork.GetRepository<Content, int>().Update(content);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<ContentResultDTO>(content);
        }

        public async Task MarkContentCompleteAsync(Guid courseId, int contentId, string userId, bool isComplete)
        {
            // Verify content belongs to this course
            var content = await unitOfWork.GetRepository<Content, int>().GetAsync(contentId)
                ?? throw new ContentNotFoundException(contentId);

            if (content.CourseId != courseId)
                throw new ContentNotFoundException(contentId);

            // Upsert WatchProgress
            var allProgress = await unitOfWork.GetRepository<WatchProgress, int>().GetAllAsync();
            var existing = allProgress.FirstOrDefault(wp => wp.UserId == userId && wp.ContentId == contentId);

            if (existing != null)
            {
                existing.IsCompleted = isComplete;
                existing.LastUpdated = EgyptDateTime.Now;
                unitOfWork.GetRepository<WatchProgress, int>().Update(existing);
            }
            else
            {
                var newProgress = new WatchProgress
                {
                    UserId = userId,
                    ContentId = contentId,
                    IsCompleted = isComplete,
                    LastPositionSeconds = 0,
                    LastUpdated = EgyptDateTime.Now
                };
                await unitOfWork.GetRepository<WatchProgress, int>().AddAsync(newProgress);
            }

            await unitOfWork.SaveChangesAsync();

            // Recalculate enrollment progress
            var enrollments = await unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var enrollment = enrollments.FirstOrDefault(e => e.CourseId == courseId && e.StudentId == userId);
            if (enrollment != null)
            {
                var courseContents = await unitOfWork.GetRepository<Content, int>().GetAllAsync();
                var visibleContents = courseContents
                    .Where(c => c.CourseId == courseId && c.IsVisible)
                    .Select(c => c.Id)
                    .ToHashSet();

                var updatedProgress = await unitOfWork.GetRepository<WatchProgress, int>().GetAllAsync();
                var completedCount = updatedProgress
                    .Count(wp => wp.UserId == userId && visibleContents.Contains(wp.ContentId) && wp.IsCompleted);

                enrollment.CompletedVideos = completedCount;
                enrollment.TotalVideos = visibleContents.Count;
                enrollment.ProgressPercentage = visibleContents.Count > 0
                    ? (int)Math.Round((double)completedCount / visibleContents.Count * 100)
                    : 0;
                enrollment.LastAccessedAt = EgyptDateTime.Now;

                if (enrollment.ProgressPercentage >= 100 && !enrollment.IsCertificateIssued)
                {
                    enrollment.IsCertificateIssued = true;
                    enrollment.CertificateIssuedAt = EgyptDateTime.Now;
                }

                unitOfWork.GetRepository<Enrollment, Guid>().Update(enrollment);
                await unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<ContentProgressDTO> GetCourseContentProgressAsync(Guid courseId, string userId)
        {
            // Verify course exists
            _ = await unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            var courseContents = await unitOfWork.GetRepository<Content, int>().GetAllAsync();
            var visibleContents = courseContents
                .Where(c => c.CourseId == courseId && c.IsVisible)
                .ToList();

            var contentIds = visibleContents.Select(c => c.Id).ToHashSet();

            var allProgress = await unitOfWork.GetRepository<WatchProgress, int>().GetAllAsync();
            var progressMap = allProgress
                .Where(wp => wp.UserId == userId && contentIds.Contains(wp.ContentId))
                .ToDictionary(wp => wp.ContentId, wp => wp.IsCompleted);

            var items = visibleContents.Select(c => new ContentCompletionStatusDTO
            {
                ContentId = c.Id,
                ContentTitle = c.Title,
                ContentType = c.Type.ToString(),
                IsCompleted = progressMap.TryGetValue(c.Id, out var done) && done
            }).ToList();

            var completedCount = items.Count(i => i.IsCompleted);

            return new ContentProgressDTO
            {
                CourseId = courseId,
                TotalContents = visibleContents.Count,
                CompletedContents = completedCount,
                ProgressPercentage = visibleContents.Count > 0
                    ? Math.Round((double)completedCount / visibleContents.Count * 100, 1)
                    : 0,
                Items = items
            };
        }
    }
}
