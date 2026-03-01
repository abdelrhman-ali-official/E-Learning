namespace Domain.Exceptions;

public sealed class LiveSessionNotFoundException : NotFoundException
{
    public LiveSessionNotFoundException(Guid sessionId)
        : base($"The live session with identifier {sessionId} was not found.")
    {
    }
}
