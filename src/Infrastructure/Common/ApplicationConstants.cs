namespace Travaloud.Infrastructure.Common;

public static class ApplicationConstants
{
    public static readonly List<string> SupportedImageFormats =
    [
        ".jpeg",
        ".jpg",
        ".png",
        ".webp",
        ".avif"
    ];
    public static readonly List<string> SupportedVideoFormats =
    [
        ".mp4",
        ".mov",
        ".avi",
        ".wmv",
        ".webm"
    ];

    public static readonly string StandardImageFormat = "image/jpeg";
    public static readonly string StandardVideoFormat = "video/mp4";
    public static readonly int MaxImageWidth = 1500;
    public static readonly int MaxImageHeight = 1500;
    public static readonly long MaxAllowedImageSize = 1000000; // Allows Max File Size of 1 Mb.
    public static readonly long MaxAllowedVideoSize = 10000000;
}