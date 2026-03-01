namespace Domain.Exceptions;

public sealed class VideoNotFoundException : NotFoundException
{
    public VideoNotFoundException(Guid videoId)
        : base($"The video with identifier {videoId} was not found.")
    {
    }
}
