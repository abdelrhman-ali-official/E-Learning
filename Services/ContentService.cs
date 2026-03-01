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
    }
}
