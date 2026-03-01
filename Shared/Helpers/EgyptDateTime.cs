namespace Shared.Helpers;


public static class EgyptDateTime
{
    private static readonly TimeZoneInfo _egyptZone =
        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

    
    public static DateTime Now =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _egyptZone);
}
