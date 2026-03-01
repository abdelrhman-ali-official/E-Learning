using Shared.VideoModels;

namespace Services.Abstractions;

/// <summary>
/// Generates a short-lived RS256-signed JWT for Jitsi as a Service (JaaS).
/// </summary>
public interface IJaaSTokenService
{
    /// <summary>
    /// Produces a <see cref="JoinLiveSessionResponseDTO"/> containing a signed JWT
    /// and the full iframe-ready room URL.
    /// </summary>
    JoinLiveSessionResponseDTO GenerateToken(
        string userId,
        string userName,
        string email,
        string roomName,
        bool isModerator);
}
