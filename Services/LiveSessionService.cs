using AutoMapper;
using Domain.Contracts;
using Domain.Entities.CourseEntities;
using Domain.Entities.VideoEntities;
using Domain.Exceptions;
using Services.Abstractions;
using Shared.Helpers;
using Shared.VideoModels;

namespace Services;

public class LiveSessionService : ILiveSessionService
{
    private readonly IUnitOFWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICourseAccessService _courseAccessService;
    private readonly IJaaSTokenService _jaasTokenService;

    public LiveSessionService(
        IUnitOFWork unitOfWork,
        IMapper mapper,
        ICourseAccessService courseAccessService,
        IJaaSTokenService jaasTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _courseAccessService = courseAccessService;
        _jaasTokenService = jaasTokenService;
    }

    public async Task<LiveSessionResponseDTO> CreateLiveSessionAsync(Guid courseId, CreateLiveSessionDTO dto, string instructorId)
    {
        // Validate course exists
        var courseRepo = _unitOfWork.GetRepository<Course, Guid>();
        var course = await courseRepo.GetAsync(courseId);
        
        if (course == null)
        {
            throw new ProductNotFoundException(courseId.ToString());
        }

        // Validate start time < end time
        if (dto.ScheduledStart >= dto.ScheduledEnd)
        {
            throw new ValidationException(new[] { "Scheduled start time must be before end time" });
        }

        var session = _mapper.Map<LiveSession>(dto);
        session.CourseId = courseId;
        session.InstructorId = instructorId;
        session.IsActive = true; // Active by default; admin can deactivate to cancel
        session.IsRecordedAvailable = false;
        session.Provider = "JITSI";
        session.CreatedAt = EgyptDateTime.Now;
        session.UpdatedAt = EgyptDateTime.Now;

        // Persist first so we have the session Id for the room name
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        await sessionRepo.AddAsync(session);
        await _unitOfWork.SaveChangesAsync();

        // Generate deterministic room name after Id is known
        session.RoomName = $"course-{courseId}-session-{session.Id}";
        sessionRepo.Update(session);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LiveSessionResponseDTO>(session);
    }

    public async Task<LiveSessionResponseDTO> UpdateLiveSessionAsync(Guid sessionId, UpdateLiveSessionDTO dto)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId);

        if (session == null)
        {
            throw new LiveSessionNotFoundException(sessionId);
        }

        // Validate start time < end time
        if (dto.ScheduledStart >= dto.ScheduledEnd)
        {
            throw new ValidationException(new[] { "Scheduled start time must be before end time" });
        }

        _mapper.Map(dto, session);
        session.UpdatedAt = EgyptDateTime.Now;
        sessionRepo.Update(session);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LiveSessionResponseDTO>(session);
    }

    public async Task DeleteLiveSessionAsync(Guid sessionId, string userId, bool isAdmin)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId);

        if (session == null)
        {
            throw new LiveSessionNotFoundException(sessionId);
        }

        // Authorization: Admin can delete any, Instructor can only delete their own
        if (!isAdmin && session.InstructorId != userId)
        {
            throw new UnAuthorizedException("You are not authorized to delete this live session");
        }

        sessionRepo.Delete(session);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ActivateLiveSessionAsync(Guid sessionId)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId);

        if (session == null)
        {
            throw new LiveSessionNotFoundException(sessionId);
        }

        session.IsActive = true;
        sessionRepo.Update(session);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateLiveSessionAsync(Guid sessionId)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId);

        if (session == null)
        {
            throw new LiveSessionNotFoundException(sessionId);
        }

        session.IsActive = false;
        sessionRepo.Update(session);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AttachRecordingAsync(Guid sessionId, AttachRecordingDTO dto)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId);

        if (session == null)
        {
            throw new LiveSessionNotFoundException(sessionId);
        }

        session.RecordingVideoId = dto.RecordingLink;
        session.IsRecordedAvailable = true;

        sessionRepo.Update(session);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeactivateLiveSessionByInstructorAsync(Guid sessionId, string instructorId)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId);

        if (session == null)
            throw new LiveSessionNotFoundException(sessionId);

        if (session.InstructorId != instructorId)
            throw new UnAuthorizedException("You are not authorized to deactivate this live session");

        session.IsActive = false;
        session.UpdatedAt = EgyptDateTime.Now;
        sessionRepo.Update(session);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AttachRecordingByInstructorAsync(Guid sessionId, string instructorId, AttachRecordingDTO dto)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId);

        if (session == null)
            throw new LiveSessionNotFoundException(sessionId);

        if (session.InstructorId != instructorId)
            throw new UnAuthorizedException("You are not authorized to attach a recording to this live session");

        session.RecordingVideoId = dto.RecordingLink;
        session.IsRecordedAvailable = true;
        session.UpdatedAt = EgyptDateTime.Now;
        sessionRepo.Update(session);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<LiveSessionResponseDTO>> GetCourseLiveSessionsAsync(Guid courseId)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var allSessions = await sessionRepo.GetAllAsync(trackChanges: false);
        
        var sessions = allSessions.Where(s => 
            s.CourseId == courseId &&
            (s.IsActive || s.ScheduledEnd >= EgyptDateTime.Now))
            .OrderBy(s => s.ScheduledStart);

        return _mapper.Map<IEnumerable<LiveSessionResponseDTO>>(sessions);
    }

    public async Task<IEnumerable<LiveSessionResponseDTO>> GetUpcomingSessionsForStudentAsync(string userId)
    {
        // Resolve courses the student is actively enrolled in
        var enrollmentRepo = _unitOfWork.GetRepository<Enrollment, Guid>();
        var allEnrollments = await enrollmentRepo.GetAllAsync(trackChanges: false);
        var enrolledCourseIds = allEnrollments
            .Where(e => e.StudentId == userId &&
                        e.IsActive &&
                        (e.ExpiresAt == null || e.ExpiresAt > EgyptDateTime.Now))
            .Select(e => e.CourseId)
            .ToHashSet();

        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var allSessions = await sessionRepo.GetAllAsync(trackChanges: false);

        var sessions = allSessions
            .Where(s => s.IsActive && enrolledCourseIds.Contains(s.CourseId))
            .OrderByDescending(s => s.ScheduledStart);

        return _mapper.Map<IEnumerable<LiveSessionResponseDTO>>(sessions);
    }

    public async Task<LiveSessionResponseDTO> GetLiveSessionByIdAsync(Guid sessionId, string userId, bool isEnrolled)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId)
            ?? throw new LiveSessionNotFoundException(sessionId);

        // Validate the student is enrolled in the session's course
        if (!isEnrolled)
            await _courseAccessService.ValidateCourseAccessAsync(session.CourseId, userId);

        if (!session.IsActive)
            throw new CourseAccessDeniedException("This live session is not currently active");

        return _mapper.Map<LiveSessionResponseDTO>(session);
    }

    public async Task<LiveStreamResponseDTO> GetLiveStreamAsync(Guid courseId, string userId, string ipAddress)
    {
        // Validate purchase/subscription
        await _courseAccessService.ValidateCourseAccessAsync(courseId, userId);

        // Get active live session for the course
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var allSessions = await sessionRepo.GetAllAsync(trackChanges: false);
        
        var activeSession = allSessions
            .Where(s => s.CourseId == courseId && s.IsActive)
            .OrderBy(s => s.ScheduledStart)
            .FirstOrDefault();

        if (activeSession == null)
        {
            throw new LiveSessionNotFoundException(Guid.Empty);
        }

        var now = EgyptDateTime.Now;

        // Check if live session is within time window
        if (now >= activeSession.ScheduledStart && now <= activeSession.ScheduledEnd)
        {
            // Live is currently active
            await LogLiveAccessAsync(userId, courseId, activeSession.Id, ipAddress);

            var liveEmbedUrl = GenerateYouTubeEmbedUrl(activeSession.YouTubeLiveVideoId);

            return new LiveStreamResponseDTO
            {
                Title = activeSession.Title,
                Description = activeSession.Description,
                IsLive = true,
                EmbedUrl = liveEmbedUrl,
                ScheduledEnd = activeSession.ScheduledEnd
            };
        }
        else if (now > activeSession.ScheduledEnd && activeSession.IsRecordedAvailable && !string.IsNullOrEmpty(activeSession.RecordingVideoId))
        {
            // Live has ended, return recording
            await LogLiveAccessAsync(userId, courseId, activeSession.Id, ipAddress);

            var recordingEmbedUrl = GenerateYouTubeEmbedUrl(activeSession.RecordingVideoId);

            return new LiveStreamResponseDTO
            {
                Title = activeSession.Title,
                Description = activeSession.Description,
                IsLive = false,
                EmbedUrl = recordingEmbedUrl,
                ScheduledEnd = null
            };
        }
        else
        {
            // Outside time window and no recording available
            throw new CourseAccessDeniedException("Live session is not currently available");
        }
    }

    public async Task DeactivateExpiredSessionsAsync()
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var allSessions = await sessionRepo.GetAllAsync(trackChanges: true);
        
        var activeSessions = allSessions.Where(s => s.IsActive && s.ScheduledEnd < EgyptDateTime.Now);

        foreach (var session in activeSessions)
        {
            session.IsActive = false;
            sessionRepo.Update(session);
        }

        if (activeSessions.Any())
        {
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task LogLiveAccessAsync(string userId, Guid courseId, Guid sessionId, string ipAddress)
    {
        var log = new VideoAccessLog
        {
            UserId = userId,
            CourseId = courseId,
            VideoId = sessionId,
            VideoType = "LiveSession",
            AccessedAt = EgyptDateTime.Now,
            IpAddress = ipAddress
        };

        var logRepo = _unitOfWork.GetRepository<VideoAccessLog, Guid>();
        await logRepo.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();
    }

    private string GenerateYouTubeEmbedUrl(string videoId)
    {
        return $"https://www.youtube.com/embed/{videoId}?rel=0&modestbranding=1";
    }

    // ── JaaS Join ─────────────────────────────────────────────────────────────

    public async Task<JoinLiveSessionResponseDTO> JoinLiveSessionAsync(
        Guid sessionId,
        string userId,
        string userName,
        string email,
        bool isModerator)
    {
        var sessionRepo = _unitOfWork.GetRepository<LiveSession, Guid>();
        var session = await sessionRepo.GetAsync(sessionId)
            ?? throw new LiveSessionNotFoundException(sessionId);

        // Session must be active
        if (!session.IsActive)
            throw new CourseAccessDeniedException("This live session is not currently active.");

        // Validate time window (allow joining 15 min early, up to 30 min after end)
        var now = EgyptDateTime.Now;
        var joinOpenTime  = session.ScheduledStart.AddMinutes(-15);
        var joinCloseTime = session.ScheduledEnd.AddMinutes(30);

        if (now < joinOpenTime)
            throw new CourseAccessDeniedException(
                $"Session has not started yet. You can join from {joinOpenTime:HH:mm}.");

        if (now > joinCloseTime)
            throw new CourseAccessDeniedException("This session has already ended.");

        // Students must be enrolled in the course
        if (!isModerator)
            await _courseAccessService.ValidateCourseAccessAsync(session.CourseId, userId);

        // Instructors: verify they own the session (or are admin — caller passes isModerator=true)
        // No extra check needed here; admin/instructor flag is set in the controller.

        var roomName = session.RoomName
            ?? $"course-{session.CourseId}-session-{session.Id}";

        return _jaasTokenService.GenerateToken(userId, userName, email, roomName, isModerator);
    }
}
