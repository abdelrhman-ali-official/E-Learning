using Domain.Entities.CourseEntities;
using Domain.Entities.SubscriptionEntities;
using Domain.Exceptions;
using Shared;
using Shared.CourseModels;
using Shared.Helpers;
using System.Text;
using System.Text.RegularExpressions;

namespace Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CourseResponseDTO> CreateCourseAsync(CreateCourseDTO dto, string instructorId, string instructorName)
        {
            // Generate slug from title
            var slug = GenerateSlug(dto.Title);
            
            // Ensure slug is unique
            slug = await EnsureUniqueSlugAsync(slug);

            var course = new Course
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Slug = slug,
                Description = dto.Description ?? string.Empty,
                ThumbnailUrl = dto.ThumbnailUrl,
                InstructorId = instructorId,
                InstructorName = instructorName,
                Price = dto.IsFree ? 0 : dto.Price,
                IsFree = dto.IsFree,
                AccessDurationDays = dto.AccessDurationDays,
                EstimatedDurationMinutes = dto.EstimatedDurationMinutes,
                Category = dto.Category,
                Level = dto.Level,
                Requirements = dto.Requirements,
                LearningObjectives = dto.LearningObjectives,
                IsPublished = false, // Created as draft
                IsFeatured = false,
                IsDeleted = false,
                CreatedAt = EgyptDateTime.Now,
                UpdatedAt = EgyptDateTime.Now
            };

            await _unitOfWork.GetRepository<Course, Guid>().AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CourseResponseDTO>(course);
        }

        public async Task<CourseResponseDTO> UpdateCourseAsync(Guid id, UpdateCourseDTO dto)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(id)
                ?? throw new CourseNotFoundException(id);

            if (course.IsDeleted)
                throw new CourseNotFoundException(id);

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.Title))
            {
                course.Title = dto.Title;
                // Regenerate slug if title changed
                var newSlug = GenerateSlug(dto.Title);
                if (newSlug != course.Slug)
                {
                    course.Slug = await EnsureUniqueSlugAsync(newSlug, course.Id);
                }
            }

            if (!string.IsNullOrEmpty(dto.Description))
                course.Description = dto.Description;

            if (dto.ThumbnailUrl != null)
                course.ThumbnailUrl = dto.ThumbnailUrl;

            if (dto.Price.HasValue)
                course.Price = dto.IsFree == true ? 0 : dto.Price.Value;

            if (dto.IsFree.HasValue)
            {
                course.IsFree = dto.IsFree.Value;
                if (course.IsFree)
                    course.Price = 0;
            }

            if (dto.AccessDurationDays.HasValue)
                course.AccessDurationDays = dto.AccessDurationDays.Value;

            if (dto.EstimatedDurationMinutes.HasValue)
                course.EstimatedDurationMinutes = dto.EstimatedDurationMinutes.Value;

            if (dto.Category != null)
                course.Category = dto.Category;

            if (dto.Level != null)
                course.Level = dto.Level;

            if (dto.Requirements != null)
                course.Requirements = dto.Requirements;

            if (dto.LearningObjectives != null)
                course.LearningObjectives = dto.LearningObjectives;

            course.UpdatedAt = EgyptDateTime.Now;

            _unitOfWork.GetRepository<Course, Guid>().Update(course);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CourseResponseDTO>(course);
        }

        // ─── Instructor-scoped ────────────────────────────────────────────────────

        /// <summary>
        /// Update the price and/or access duration of a course.
        /// Only the instructor who owns the course may call this.
        /// </summary>
        public async Task<CourseResponseDTO> UpdateCoursePricingAsync(Guid courseId, string instructorId, UpdateCoursePricingDTO dto)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            if (course.IsDeleted)
                throw new CourseNotFoundException(courseId);

            if (course.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to update this course's pricing");

            if (dto.IsFree.HasValue)
            {
                course.IsFree = dto.IsFree.Value;
                if (course.IsFree) course.Price = 0;
            }

            if (dto.Price.HasValue && !course.IsFree)
                course.Price = dto.Price.Value;

            if (dto.AccessDurationDays.HasValue)
                course.AccessDurationDays = dto.AccessDurationDays.Value;

            course.UpdatedAt = EgyptDateTime.Now;
            _unitOfWork.GetRepository<Course, Guid>().Update(course);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CourseResponseDTO>(course);
        }

        /// <summary>
        /// Get all courses belonging to a specific instructor.
        /// </summary>
        public async Task<PaginatedResult<CourseResponseDTO>> GetInstructorCoursesAsync(string instructorId, int pageIndex = 1, int pageSize = 10)
        {
            var allCourses = await _unitOfWork.GetRepository<Course, Guid>().GetAllAsync();

            var filtered = allCourses
                .Where(c => !c.IsDeleted && c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            var totalCount = filtered.Count;
            var paged = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            var courseIds = paged.Select(c => c.Id).ToHashSet();

            // Load enrollment counts
            var allEnrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var enrollmentCounts = allEnrollments
                .Where(e => courseIds.Contains(e.CourseId) && e.IsActive)
                .GroupBy(e => e.CourseId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Load review stats
            var allReviews = await _unitOfWork.GetRepository<CourseReview, Guid>().GetAllAsync();
            var reviewStats = allReviews
                .Where(r => courseIds.Contains(r.CourseId) && r.IsApproved && !r.IsHidden)
                .GroupBy(r => r.CourseId)
                .ToDictionary(g => g.Key, g => (Count: g.Count(), Avg: g.Average(r => (double)r.Rating)));

            var courseDtos = paged.Select(c =>
            {
                var dto = _mapper.Map<CourseResponseDTO>(c);
                var enrolled      = enrollmentCounts.TryGetValue(c.Id, out var ec) ? ec : 0;
                var reviewCount   = reviewStats.TryGetValue(c.Id, out var rd) ? rd.Count : 0;
                var averageRating = reviewStats.TryGetValue(c.Id, out var rd2) ? Math.Round(rd2.Avg, 2) : 0.0;
                return dto with
                {
                    TotalEnrollments = enrolled,
                    TotalReviews     = reviewCount,
                    AverageRating    = averageRating
                };
            }).ToList();

            return new PaginatedResult<CourseResponseDTO>(pageIndex, pageSize, totalCount, courseDtos);
        }

        // ─── Admin CRUD ───────────────────────────────────────────────────────────

        public async Task DeleteCourseAsync(Guid id)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(id)
                ?? throw new CourseNotFoundException(id);

            // Soft delete
            course.IsDeleted = true;
            course.DeletedAt = EgyptDateTime.Now;
            course.UpdatedAt = EgyptDateTime.Now;

            _unitOfWork.GetRepository<Course, Guid>().Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CourseDetailDTO> GetCourseByIdAsync(Guid id)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(id)
                ?? throw new CourseNotFoundException(id);

            if (course.IsDeleted)
                throw new CourseNotFoundException(id);

            return _mapper.Map<CourseDetailDTO>(course);
        }

        public async Task<CourseDetailDTO> GetCourseBySlugAsync(string slug, string? studentId = null)
        {
            var courses = await _unitOfWork.GetRepository<Course, Guid>().GetAllAsync();
            var course = courses.FirstOrDefault(c => c.Slug == slug && !c.IsDeleted)
                ?? throw new CourseNotFoundException(slug);

            var dto = _mapper.Map<CourseDetailDTO>(course);

            if (!string.IsNullOrEmpty(studentId))
            {
                var enrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
                var isEnrolled = enrollments.Any(e =>
                    e.CourseId == course.Id &&
                    e.StudentId == studentId &&
                    e.IsActive);
                dto = dto with { IsEnrolled = isEnrolled };
            }

            return dto;
        }

        public async Task<PaginatedResult<CourseResponseDTO>> GetAllCoursesAsync(int pageIndex = 1, int pageSize = 10, bool includeUnpublished = false)
        {
            var allCourses = await _unitOfWork.GetRepository<Course, Guid>().GetAllAsync();

            var filteredCourses = allCourses.Where(c => !c.IsDeleted);

            if (!includeUnpublished)
                filteredCourses = filteredCourses.Where(c => c.IsPublished);

            var totalCount = filteredCourses.Count();

            var pagedCourses = filteredCourses
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            var courseDtos = _mapper.Map<IEnumerable<CourseResponseDTO>>(pagedCourses);

            return new PaginatedResult<CourseResponseDTO>(
                pageIndex,
                courseDtos.Count(),
                totalCount,
                courseDtos);
        }

        public async Task<PaginatedResult<CourseResponseDTO>> GetPublishedCoursesAsync(int pageIndex = 1, int pageSize = 10, string? category = null, string? level = null, string? studentId = null)
        {
            var allCourses = await _unitOfWork.GetRepository<Course, Guid>().GetAllAsync();

            var filteredCourses = allCourses
                .Where(c => !c.IsDeleted && c.IsPublished);

            if (!string.IsNullOrEmpty(category))
                filteredCourses = filteredCourses.Where(c => c.Category == category);

            if (!string.IsNullOrEmpty(level))
                filteredCourses = filteredCourses.Where(c => c.Level == level);

            var totalCount = filteredCourses.Count();

            var pagedCourses = filteredCourses
                .OrderByDescending(c => c.IsFeatured)
                .ThenByDescending(c => c.PublishedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Resolve enrolled course IDs for this student in one query
            HashSet<Guid> enrolledIds = new();
            if (!string.IsNullOrEmpty(studentId))
            {
                var enrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
                enrolledIds = enrollments
                    .Where(e => e.StudentId == studentId && e.IsActive)
                    .Select(e => e.CourseId)
                    .ToHashSet();
            }

            var courseDtos = pagedCourses.Select(c =>
            {
                var dto = _mapper.Map<CourseResponseDTO>(c);
                return dto with { IsEnrolled = enrolledIds.Contains(c.Id) };
            }).ToList();

            return new PaginatedResult<CourseResponseDTO>(
                pageIndex,
                courseDtos.Count,
                totalCount,
                courseDtos);
        }

        public async Task PublishCourseAsync(Guid id)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(id)
                ?? throw new CourseNotFoundException(id);

            if (course.IsDeleted)
                throw new CourseNotFoundException(id);

            course.IsPublished = true;
            course.PublishedAt = EgyptDateTime.Now;
            course.UpdatedAt = EgyptDateTime.Now;

            _unitOfWork.GetRepository<Course, Guid>().Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnpublishCourseAsync(Guid id)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(id)
                ?? throw new CourseNotFoundException(id);

            if (course.IsDeleted)
                throw new CourseNotFoundException(id);

            course.IsPublished = false;
            course.UpdatedAt = EgyptDateTime.Now;

            _unitOfWork.GetRepository<Course, Guid>().Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ToggleFeatureAsync(Guid id)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(id)
                ?? throw new CourseNotFoundException(id);

            if (course.IsDeleted)
                throw new CourseNotFoundException(id);

            course.IsFeatured = !course.IsFeatured;
            course.UpdatedAt = EgyptDateTime.Now;

            _unitOfWork.GetRepository<Course, Guid>().Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CourseAccessDTO> CheckCourseAccessAsync(Guid courseId, string userId)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            if (course.IsDeleted)
                throw new CourseNotFoundException(courseId);

            if (!course.IsPublished)
            {
                return new CourseAccessDTO
                {
                    HasAccess = false,
                    DeniedReason = "Course is not published yet"
                };
            }

            // Check if free course
            if (course.IsFree)
            {
                return new CourseAccessDTO
                {
                    HasAccess = true,
                    AccessReason = "Free",
                    ExpiresAt = null
                };
            }

            // Check enrollment
            var enrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var enrollment = enrollments.FirstOrDefault(e => 
                e.CourseId == courseId && 
                e.StudentId == userId && 
                e.IsActive);

            if (enrollment != null)
            {
                if (enrollment.ExpiresAt.HasValue && enrollment.ExpiresAt.Value < EgyptDateTime.Now)
                {
                    return new CourseAccessDTO
                    {
                        HasAccess = false,
                        DeniedReason = "Enrollment expired"
                    };
                }

                return new CourseAccessDTO
                {
                    HasAccess = true,
                    AccessReason = enrollment.Source.ToString(),
                    ExpiresAt = enrollment.ExpiresAt
                };
            }

            // Check active subscription
            var subscriptions = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAllAsync();
            var activeSubscription = subscriptions.FirstOrDefault(s => 
                s.StudentId == userId && 
                s.Status == SubscriptionStatus.Active &&
                s.EndDate > EgyptDateTime.Now);

            if (activeSubscription != null)
            {
                return new CourseAccessDTO
                {
                    HasAccess = true,
                    AccessReason = "Subscription",
                    ExpiresAt = activeSubscription.EndDate
                };
            }

            return new CourseAccessDTO
            {
                HasAccess = false,
                DeniedReason = "No active enrollment or subscription"
            };
        }

        public async Task<CourseReviewResponseDTO> SubmitReviewAsync(Guid courseId, string userId, string userName, CourseReviewDTO dto)
        {
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            if (course.IsDeleted)
                throw new CourseNotFoundException(courseId);

            // Check if user already reviewed this course
            var reviews = await _unitOfWork.GetRepository<CourseReview, Guid>().GetAllAsync();
            var existingReview = reviews.FirstOrDefault(r => r.CourseId == courseId && r.StudentId == userId);

            if (existingReview != null)
            {
                // Update existing review
                existingReview.Rating = dto.Rating;
                existingReview.ReviewText = dto.ReviewText;
                existingReview.UpdatedAt = EgyptDateTime.Now;

                _unitOfWork.GetRepository<CourseReview, Guid>().Update(existingReview);
                await _unitOfWork.SaveChangesAsync();

                return new CourseReviewResponseDTO
                {
                    Id = existingReview.Id,
                    CourseId = existingReview.CourseId,
                    StudentId = existingReview.StudentId,
                    StudentName = userName,
                    Rating = existingReview.Rating,
                    ReviewText = existingReview.ReviewText,
                    CreatedAt = existingReview.CreatedAt
                };
            }

            // Create new review
            var review = new CourseReview
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                StudentId = userId,
                Rating = dto.Rating,
                ReviewText = dto.ReviewText,
                IsApproved = true,
                IsHidden = false,
                CreatedAt = EgyptDateTime.Now,
                UpdatedAt = EgyptDateTime.Now
            };

            await _unitOfWork.GetRepository<CourseReview, Guid>().AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return new CourseReviewResponseDTO
            {
                Id = review.Id,
                CourseId = review.CourseId,
                StudentId = review.StudentId,
                StudentName = userName,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<IEnumerable<CourseReviewResponseDTO>> GetCourseReviewsAsync(Guid courseId)
        {
            var reviews = await _unitOfWork.GetRepository<CourseReview, Guid>().GetAllAsync();
            var courseReviews = reviews
                .Where(r => r.CourseId == courseId && r.IsApproved && !r.IsHidden)
                .OrderByDescending(r => r.CreatedAt);

            return _mapper.Map<IEnumerable<CourseReviewResponseDTO>>(courseReviews);
        }

        // Helper methods
        private string GenerateSlug(string title)
        {
            // Convert to lowercase
            var slug = title.ToLowerInvariant();

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Convert spaces to hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            // Remove duplicate hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Trim hyphens from ends
            slug = slug.Trim('-');

            // Limit length
            if (slug.Length > 100)
                slug = slug.Substring(0, 100).TrimEnd('-');

            return slug;
        }

        private async Task<string> EnsureUniqueSlugAsync(string slug, Guid? excludeId = null)
        {
            var allCourses = await _unitOfWork.GetRepository<Course, Guid>().GetAllAsync();
            var existingSlugs = allCourses
                .Where(c => !c.IsDeleted && (!excludeId.HasValue || c.Id != excludeId.Value))
                .Select(c => c.Slug)
                .ToList();

            if (!existingSlugs.Contains(slug))
                return slug;

            // Append number to make unique
            var counter = 1;
            var uniqueSlug = $"{slug}-{counter}";

            while (existingSlugs.Contains(uniqueSlug))
            {
                counter++;
                uniqueSlug = $"{slug}-{counter}";
            }

            return uniqueSlug;
        }
    }
}
