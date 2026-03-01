using Domain.Entities.CourseEntities;
using Domain.Entities.SubscriptionEntities;
using Domain.Exceptions;
using Shared.CourseModels;
using Shared.Helpers;

namespace Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOFWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOFWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<EnrollmentResponseDTO> EnrollStudentAsync(Guid courseId, string studentId, string studentName)
        {
            // Verify course exists and is published
            var course = await _unitOfWork.GetRepository<Course, Guid>().GetAsync(courseId)
                ?? throw new CourseNotFoundException(courseId);

            if (course.IsDeleted)
                throw new CourseNotFoundException(courseId);

            if (!course.IsPublished)
                throw new CourseNotPublishedException(courseId);

            // Check if already enrolled
            var enrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var existingEnrollment = enrollments.FirstOrDefault(e => 
                e.CourseId == courseId && 
                e.StudentId == studentId);

            if (existingEnrollment != null)
            {
                // Reactivate if inactive
                if (!existingEnrollment.IsActive)
                {
                    existingEnrollment.IsActive = true;
                    existingEnrollment.LastAccessedAt = EgyptDateTime.Now;
                    _unitOfWork.GetRepository<Enrollment, Guid>().Update(existingEnrollment);
                    await _unitOfWork.SaveChangesAsync();
                }

                return _mapper.Map<EnrollmentResponseDTO>(existingEnrollment);
            }

            // Determine enrollment source
            var source = EnrollmentSource.Free; // Default for free courses
            DateTime? expiresAt = null;

            if (!course.IsFree)
            {
                // Check for active subscription
                var subscriptions = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAllAsync();
                var activeSubscription = subscriptions.FirstOrDefault(s => 
                    s.StudentId == studentId && 
                    s.Status == SubscriptionStatus.Active &&
                    s.EndDate > EgyptDateTime.Now);

                if (activeSubscription != null)
                {
                    source = EnrollmentSource.Subscription;
                    // Use the course's own access duration (0 = unlimited)
                    expiresAt = course.AccessDurationDays > 0
                        ? EgyptDateTime.Now.AddDays(course.AccessDurationDays)
                        : (DateTime?)null;
                }
                else
                {
                    throw new UnAuthorizedException("You must purchase this course or have an active subscription to enroll");
                }
            }

            // Create enrollment
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                StudentId = studentId,
                Source = source,
                EnrolledAt = EgyptDateTime.Now,
                ExpiresAt = expiresAt,
                IsActive = true,
                ProgressPercentage = 0,
                CompletedVideos = 0,
                TotalVideos = 0,
                LastAccessedAt = EgyptDateTime.Now,
                IsCertificateIssued = false
            };

            await _unitOfWork.GetRepository<Enrollment, Guid>().AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EnrollmentResponseDTO>(enrollment);
        }

        public async Task<EnrollmentResponseDTO?> GetEnrollmentAsync(Guid courseId, string studentId)
        {
            var enrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var enrollment = enrollments.FirstOrDefault(e => 
                e.CourseId == courseId && 
                e.StudentId == studentId);

            return enrollment != null ? _mapper.Map<EnrollmentResponseDTO>(enrollment) : null;
        }

        public async Task<IEnumerable<EnrollmentResponseDTO>> GetStudentEnrollmentsAsync(string studentId)
        {
            var enrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var studentEnrollments = enrollments
                .Where(e => e.StudentId == studentId && e.IsActive)
                .OrderByDescending(e => e.LastAccessedAt);

            return _mapper.Map<IEnumerable<EnrollmentResponseDTO>>(studentEnrollments);
        }

        public async Task UpdateProgressAsync(Guid enrollmentId, int completedVideos, int totalVideos)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAsync(enrollmentId)
                ?? throw new EnrollmentNotFoundException(enrollmentId);

            enrollment.CompletedVideos = completedVideos;
            enrollment.TotalVideos = totalVideos;
            enrollment.ProgressPercentage = totalVideos > 0 
                ? (int)Math.Round((double)completedVideos / totalVideos * 100) 
                : 0;
            enrollment.LastAccessedAt = EgyptDateTime.Now;

            // Auto-issue certificate if 100% complete
            if (enrollment.ProgressPercentage >= 100 && !enrollment.IsCertificateIssued)
            {
                enrollment.IsCertificateIssued = true;
                enrollment.CertificateIssuedAt = EgyptDateTime.Now;
            }

            _unitOfWork.GetRepository<Enrollment, Guid>().Update(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task IssueCertificateAsync(Guid enrollmentId)
        {
            var enrollment = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAsync(enrollmentId)
                ?? throw new EnrollmentNotFoundException(enrollmentId);

            if (enrollment.ProgressPercentage < 100)
                throw new ValidationException(new[] { "Student must complete 100% of the course to receive a certificate" });

            enrollment.IsCertificateIssued = true;
            enrollment.CertificateIssuedAt = EgyptDateTime.Now;

            _unitOfWork.GetRepository<Enrollment, Guid>().Update(enrollment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> HasAccessToCourseAsync(Guid courseId, string studentId)
        {
            // Check enrollment
            var enrollments = await _unitOfWork.GetRepository<Enrollment, Guid>().GetAllAsync();
            var enrollment = enrollments.FirstOrDefault(e => 
                e.CourseId == courseId && 
                e.StudentId == studentId && 
                e.IsActive);

            if (enrollment != null)
            {
                // Check if expired
                if (!enrollment.ExpiresAt.HasValue || enrollment.ExpiresAt.Value > EgyptDateTime.Now)
                    return true;
            }

            // Check active subscription
            var subscriptions = await _unitOfWork.GetRepository<StudentSubscription, Guid>().GetAllAsync();
            var activeSubscription = subscriptions.FirstOrDefault(s => 
                s.StudentId == studentId && 
                s.Status == SubscriptionStatus.Active &&
                s.EndDate > EgyptDateTime.Now);

            return activeSubscription != null;
        }
    }
}
