namespace Shared.VideoModels;

public record JoinLiveSessionResponseDTO
{
    public string Jwt { get; init; } = string.Empty;

    
    public string RoomUrl { get; init; } = string.Empty;
}
