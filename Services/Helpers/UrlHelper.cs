using System.Text.RegularExpressions;

namespace Services.Helpers;

/// <summary>
/// Helper class for URL validation and transformation
/// </summary>
public static class UrlHelper
{
    /// <summary>
    /// Validates if a URL is a valid URL format
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Auto-prepend https:// for URLs starting with www.
        var normalizedUrl = url;
        if (url.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            normalizedUrl = "https://" + url;

        return Uri.TryCreate(normalizedUrl, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Converts Google Drive view/edit URL to embeddable preview URL
    /// From: https://drive.google.com/file/d/FILE_ID/view or /edit
    /// To: https://drive.google.com/file/d/FILE_ID/preview
    /// </summary>
    public static string ConvertGoogleDriveToPreview(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return url;

        // Pattern to match Google Drive URLs
        var patterns = new[]
        {
            @"https://drive\.google\.com/file/d/([a-zA-Z0-9_-]+)/(view|edit)",
            @"https://drive\.google\.com/open\?id=([a-zA-Z0-9_-]+)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(url, pattern);
            if (match.Success)
            {
                var fileId = match.Groups[1].Value;
                return $"https://drive.google.com/file/d/{fileId}/preview";
            }
        }

        // If URL is already in correct format or not a Google Drive URL, return as is
        return url;
    }

    /// <summary>
    /// Returns true when the URL points to a YouTube video.
    /// </summary>
    public static bool IsYouTubeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        return url.Contains("youtube.com", StringComparison.OrdinalIgnoreCase)
            || url.Contains("youtu.be", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extracts the bare video ID from common YouTube URL formats.
    /// Returns null if no ID can be found.
    /// </summary>
    public static string? ExtractYouTubeVideoId(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return null;

        // youtu.be/VIDEO_ID
        var shortMatch = Regex.Match(url, @"youtu\.be/([a-zA-Z0-9_-]{11})");
        if (shortMatch.Success) return shortMatch.Groups[1].Value;

        // youtube.com/watch?v=VIDEO_ID  or  /embed/VIDEO_ID
        var longMatch = Regex.Match(url, @"[?&v=|/embed/]([a-zA-Z0-9_-]{11})");
        if (longMatch.Success) return longMatch.Groups[1].Value;

        return null;
    }

    /// <summary>
    /// Validates if URL is a Google Drive URL
    /// </summary>
    public static bool IsGoogleDriveUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return url.Contains("drive.google.com", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates if URL is a meeting link (Zoom, Teams, Google Meet, etc.)
    /// </summary>
    public static bool IsMeetingLink(string url)
    {
        if (!IsValidUrl(url))
            return false;

        var meetingDomains = new[]
        {
            "zoom.us",
            "meet.google.com",
            "teams.microsoft.com",
            "webex.com",
            "gotomeeting.com",
            "meet.jit.si"
        };

        try
        {
            var uri = new Uri(url);
            return meetingDomains.Any(domain => uri.Host.Contains(domain, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }
}
