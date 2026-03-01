using Domain.Entities.CourseEntities;
using Domain.Entities.SecurityEntities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Shared;
using Shared.CourseModels;
using Shared.Helpers;

namespace Services
{
    public class CoursePaymentService(
        IUnitOFWork unitOfWork,
        IMapper mapper,
        UserManager<User> userManager) : ICoursePaymentService
    {

        public async Task<EWalletMethodDTO> AddEWalletMethodAsync(Guid courseId, string instructorId, AddEWalletMethodDTO dto)
        {
            var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            if (course.IsDeleted)
                throw new CourseNotFoundException(courseId);

            if (course.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to manage payment methods for this course");

            var method = new CourseEWalletMethod
            {
                CourseId    = courseId,
                InstructorId = instructorId,
                MethodName  = dto.MethodName,
                WalletNumber = dto.WalletNumber,
                IsActive    = true,
                CreatedAt   = EgyptDateTime.Now
            };

            await unitOfWork.GetRepository<CourseEWalletMethod, int>().AddAsync(method);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<EWalletMethodDTO>(method);
        }

        public async Task<EWalletMethodDTO> UpdateEWalletMethodAsync(int methodId, string instructorId, AddEWalletMethodDTO dto)
        {
            var method = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAsync(methodId)
                ?? throw new ManualPaymentRequestNotFoundException(methodId);

            if (method.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to update this payment method");

            method.MethodName   = dto.MethodName;
            method.WalletNumber = dto.WalletNumber;

            unitOfWork.GetRepository<CourseEWalletMethod, int>().Update(method);
            await unitOfWork.SaveChangesAsync();

            return mapper.Map<EWalletMethodDTO>(method);
        }

        public async Task RemoveEWalletMethodAsync(int methodId, string instructorId)
        {
            var method = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAsync(methodId)
                ?? throw new ManualPaymentRequestNotFoundException(methodId);

            if (method.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to remove this payment method");

            method.IsActive = false;
            unitOfWork.GetRepository<CourseEWalletMethod, int>().Update(method);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<EWalletMethodDTO>> GetCourseEWalletMethodsAsync(Guid courseId)
        {
            var all = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAllAsync();
            var methods = all.Where(m => m.CourseId == courseId && m.IsActive)
                             .OrderBy(m => m.MethodName);
            return mapper.Map<IEnumerable<EWalletMethodDTO>>(methods);
        }


        public async Task<CoursePaymentRequestDTO> SubmitPaymentRequestAsync(Guid courseId, string studentId, SubmitCoursePaymentDTO dto, string screenshotUrl)
        {
            var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            if (course.IsDeleted || !course.IsPublished)
                throw new CourseNotFoundException(courseId);

            var method = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAsync(dto.PaymentMethod)
                ?? throw new EWalletMethodNotFoundException(dto.PaymentMethod);

            if (method.CourseId != courseId || !method.IsActive)
                throw new ValidationException(new[] { "Selected payment method is not available for this course" });

            var existingRequests = await unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAllAsync();
            var hasPending = existingRequests.Any(r =>
                r.CourseId == courseId &&
                r.StudentId == studentId &&
                r.Status == CoursePaymentStatus.Pending);

            if (hasPending)
                throw new ValidationException(new[] { "You already have a pending payment request for this course" });

            // Prevent re-enrollment (already enrolled and active)
            var enrollments = await unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var activeEnrollment = enrollments.FirstOrDefault(e =>
                e.CourseId == courseId &&
                e.StudentId == studentId &&
                e.IsActive &&
                (e.ExpiresAt == null || e.ExpiresAt > EgyptDateTime.Now));

            if (activeEnrollment != null)
                throw new ValidationException(new[] { "You are already enrolled in this course" });

            var request = new CoursePaymentRequest
            {
                CourseId             = courseId,
                StudentId            = studentId,
                EWalletMethodId      = dto.PaymentMethod,
                Amount               = course.Price,
                StudentWalletNumber  = dto.StudentWalletNumber,
                ScreenshotUrl        = screenshotUrl,
                Status               = CoursePaymentStatus.Pending,
                CreatedAt            = EgyptDateTime.Now
            };

            await unitOfWork.GetRepository<CoursePaymentRequest, int>().AddAsync(request);
            await unitOfWork.SaveChangesAsync();

            return MapToDTO(request, course.Title, method);
        }

        public async Task<IEnumerable<CoursePaymentRequestDTO>> GetMyPaymentRequestsAsync(string studentId)
        {
            var allRequests = await unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAllAsync();
            var mine = allRequests
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            // Load related data
            var courseIds = mine.Select(r => r.CourseId).Distinct().ToList();
            var allCourses = await unitOfWork.GetRepository<Course, Guid>().GetAllAsync();
            var courseMap  = allCourses.Where(c => courseIds.Contains(c.Id))
                                       .ToDictionary(c => c.Id, c => c.Title);

            var methodIds  = mine.Select(r => r.EWalletMethodId).Distinct().ToList();
            var allMethods = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAllAsync();
            var methodMap  = allMethods.Where(m => methodIds.Contains(m.Id))
                                       .ToDictionary(m => m.Id);

            return mine.Select(r => MapToDTO(
                r,
                courseMap.TryGetValue(r.CourseId, out var ct) ? ct : "Unknown",
                methodMap.TryGetValue(r.EWalletMethodId, out var m) ? m : null));
        }


        public async Task<PaginatedResult<CoursePaymentRequestDTO>> GetCoursePaymentRequestsAsync(
            Guid courseId, string instructorId, string? status, int pageIndex, int pageSize)
        {
            var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            if (course.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to view payments for this course");

            var allRequests = await unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAllAsync();
            var allMethods  = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAllAsync();
            var methodMap   = allMethods.ToDictionary(m => m.Id);

            CoursePaymentStatus? statusFilter = status?.ToLower() switch
            {
                "pending"  => CoursePaymentStatus.Pending,
                "approved" => CoursePaymentStatus.Approved,
                "rejected" => CoursePaymentStatus.Rejected,
                _          => null
            };

            var filtered = allRequests
                .Where(r => r.CourseId == courseId)
                .Where(r => statusFilter == null || r.Status == statusFilter)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var total = filtered.Count;
            var paged = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            var studentIds = paged.Select(r => r.StudentId).Distinct().ToList();
            var studentMap = userManager.Users
                .Where(u => studentIds.Contains(u.Id))
                .Select(u => new { u.Id, u.DisplayName, u.Email })
                .ToDictionary(u => u.Id);

            var dtos = paged.Select(r =>
            {
                studentMap.TryGetValue(r.StudentId, out var student);
                return MapToDTO(
                    r,
                    course.Title,
                    methodMap.TryGetValue(r.EWalletMethodId, out var m) ? m : null,
                    student?.DisplayName ?? string.Empty,
                    student?.Email ?? string.Empty);
            });

            return new PaginatedResult<CoursePaymentRequestDTO>(pageIndex, pageSize, total, dtos);
        }

        public async Task<CoursePaymentRequestDTO> GetCoursePaymentRequestByIdAsync(int requestId, string instructorId)
        {
            var request = await unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAsync(requestId)
                ?? throw new ManualPaymentRequestNotFoundException(requestId);

            var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(request.CourseId)
                ?? throw new CourseNotFoundException(request.CourseId);

            if (course.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to view this payment request");

            var method  = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAsync(request.EWalletMethodId);
            var student = await userManager.FindByIdAsync(request.StudentId);
            return MapToDTO(request, course.Title, method, student?.DisplayName ?? string.Empty, student?.Email ?? string.Empty);
        }

        public async Task<CoursePaymentRequestDTO> InstructorReviewPaymentRequestAsync(
            int requestId, string instructorId, ReviewCoursePaymentDTO dto)
        {
            var request = await unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAsync(requestId)
                ?? throw new ManualPaymentRequestNotFoundException(requestId);

            var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(request.CourseId)
                ?? throw new CourseNotFoundException(request.CourseId);

            if (course.InstructorId != instructorId)
                throw new UnAuthorizedException("You are not authorized to review this payment request");

            if (request.Status != CoursePaymentStatus.Pending)
                throw new ValidationException(new[] { "This request has already been reviewed" });

            request.Status          = dto.Approve ? CoursePaymentStatus.Approved : CoursePaymentStatus.Rejected;
            request.ReviewedAt      = EgyptDateTime.Now;
            request.ReviewedBy      = instructorId;
            request.RejectionReason = dto.Approve ? null : dto.RejectionReason;

            unitOfWork.GetRepository<CoursePaymentRequest, int>().Update(request);

            if (dto.Approve)
            {
                var enrollments = await unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
                var existing = enrollments.FirstOrDefault(e =>
                    e.CourseId == request.CourseId && e.StudentId == request.StudentId);

                if (existing != null)
                {
                    if (!existing.IsActive || (existing.ExpiresAt.HasValue && existing.ExpiresAt <= EgyptDateTime.Now))
                    {
                        existing.IsActive       = true;
                        existing.EnrolledAt     = EgyptDateTime.Now;
                        existing.LastAccessedAt = EgyptDateTime.Now;
                        existing.ExpiresAt      = course.AccessDurationDays > 0
                            ? EgyptDateTime.Now.AddDays(course.AccessDurationDays) : null;
                        unitOfWork.GetRepository<Enrollment, Guid>().Update(existing);
                    }
                }
                else
                {
                    var enrollment = new Enrollment
                    {
                        Id              = Guid.NewGuid(),
                        CourseId        = request.CourseId,
                        StudentId       = request.StudentId,
                        Source          = EnrollmentSource.Purchase,
                        EnrolledAt      = EgyptDateTime.Now,
                        LastAccessedAt  = EgyptDateTime.Now,
                        ExpiresAt       = course.AccessDurationDays > 0
                            ? EgyptDateTime.Now.AddDays(course.AccessDurationDays) : null,
                        IsActive        = true
                    };
                    await unitOfWork.GetRepository<Enrollment, Guid>().AddAsync(enrollment);
                }
            }

            await unitOfWork.SaveChangesAsync();

            var method  = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAsync(request.EWalletMethodId);
            var student = await userManager.FindByIdAsync(request.StudentId);
            return MapToDTO(request, course.Title, method, student?.DisplayName ?? string.Empty, student?.Email ?? string.Empty);
        }


        public async Task<CoursePaymentRequestDTO> ReviewPaymentRequestAsync(int requestId, string reviewerId, ReviewCoursePaymentDTO dto)
        {
            var request = await unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAsync(requestId)
                ?? throw new ManualPaymentRequestNotFoundException(requestId);

            if (request.Status != CoursePaymentStatus.Pending)
                throw new ValidationException(new[] { "This request has already been reviewed" });

            request.Status       = dto.Approve ? CoursePaymentStatus.Approved : CoursePaymentStatus.Rejected;
            request.ReviewedAt   = EgyptDateTime.Now;
            request.ReviewedBy   = reviewerId;
            request.RejectionReason = dto.Approve ? null : dto.RejectionReason;

            unitOfWork.GetRepository<CoursePaymentRequest, int>().Update(request);

            if (dto.Approve)
            {
                var course = await unitOfWork.GetRepository<Course, Guid>().GetAsync(request.CourseId)
                    ?? throw new CourseNotFoundException(request.CourseId);

                var enrollments = await unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
                var existing = enrollments.FirstOrDefault(e =>
                    e.CourseId == request.CourseId && e.StudentId == request.StudentId);

                if (existing != null)
                {
                    if (!existing.IsActive || (existing.ExpiresAt.HasValue && existing.ExpiresAt <= EgyptDateTime.Now))
                    {
                        existing.IsActive       = true;
                        existing.EnrolledAt     = EgyptDateTime.Now;
                        existing.LastAccessedAt = EgyptDateTime.Now;
                        existing.ExpiresAt      = course.AccessDurationDays > 0
                            ? EgyptDateTime.Now.AddDays(course.AccessDurationDays) : null;
                        unitOfWork.GetRepository<Enrollment, Guid>().Update(existing);
                    }
                }
                else
                {
                    var enrollment = new Enrollment
                    {
                        Id              = Guid.NewGuid(),
                        CourseId        = request.CourseId,
                        StudentId       = request.StudentId,
                        Source          = EnrollmentSource.Purchase,
                        EnrolledAt      = EgyptDateTime.Now,
                        LastAccessedAt  = EgyptDateTime.Now,
                        ExpiresAt       = course.AccessDurationDays > 0
                            ? EgyptDateTime.Now.AddDays(course.AccessDurationDays) : null,
                        IsActive        = true
                    };
                    await unitOfWork.GetRepository<Enrollment, Guid>().AddAsync(enrollment);
                }
            }

            await unitOfWork.SaveChangesAsync();

            var courseTitle = (await unitOfWork.GetRepository<Course, Guid>().GetAsync(request.CourseId))?.Title ?? "";
            var method      = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAsync(request.EWalletMethodId);
            return MapToDTO(request, courseTitle, method);
        }

        public async Task<PaginatedResult<CoursePaymentRequestDTO>> GetPendingPaymentRequestsAsync(int pageIndex = 1, int pageSize = 20)
            => await GetRequestsAsync(r => r.Status == CoursePaymentStatus.Pending, pageIndex, pageSize);

        public async Task<PaginatedResult<CoursePaymentRequestDTO>> GetAllPaymentRequestsAsync(int pageIndex = 1, int pageSize = 20)
            => await GetRequestsAsync(_ => true, pageIndex, pageSize);


        private async Task<PaginatedResult<CoursePaymentRequestDTO>> GetRequestsAsync(
            Func<CoursePaymentRequest, bool> filter, int pageIndex, int pageSize)
        {
            var allRequests = await unitOfWork.GetRepository<CoursePaymentRequest, int>().GetAllAsync();
            var allCourses  = await unitOfWork.GetRepository<Course, Guid>().GetAllAsync();
            var allMethods  = await unitOfWork.GetRepository<CourseEWalletMethod, int>().GetAllAsync();

            var courseMap = allCourses.ToDictionary(c => c.Id, c => c.Title);
            var methodMap = allMethods.ToDictionary(m => m.Id);

            var filtered = allRequests.Where(filter).OrderByDescending(r => r.CreatedAt).ToList();
            var total    = filtered.Count;
            var paged    = filtered.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var dtos = paged.Select(r => MapToDTO(
                r,
                courseMap.TryGetValue(r.CourseId, out var ct) ? ct : "Unknown",
                methodMap.TryGetValue(r.EWalletMethodId, out var m) ? m : null));

            return new PaginatedResult<CoursePaymentRequestDTO>(pageIndex, pageSize, total, dtos);
        }

        private static CoursePaymentRequestDTO MapToDTO(
            CoursePaymentRequest r,
            string courseTitle,
            CourseEWalletMethod? method,
            string studentName = "",
            string studentEmail = "")
        {
            return new CoursePaymentRequestDTO
            {
                Id                     = r.Id,
                CourseId               = r.CourseId,
                CourseTitle            = courseTitle,
                StudentId              = r.StudentId,
                StudentName            = studentName,
                StudentEmail           = studentEmail,
                MethodName             = method?.MethodName ?? "",
                InstructorWalletNumber = method?.WalletNumber ?? "",
                StudentWalletNumber    = r.StudentWalletNumber,
                ScreenshotUrl          = r.ScreenshotUrl,
                Amount                 = r.Amount,
                Status                 = r.Status.ToString(),
                CreatedAt              = r.CreatedAt,
                ReviewedAt             = r.ReviewedAt,
                ReviewedBy             = r.ReviewedBy,
                RejectionReason        = r.RejectionReason
            };
        }
    }
}
