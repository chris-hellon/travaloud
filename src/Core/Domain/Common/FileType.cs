using System.ComponentModel;

namespace Travaloud.Domain.Common;

public enum FileType
{
    [Description(".jpg,.png,.jpeg,.webp,.avif")]
    Image,
    [Description(".mp4,.mov,.avi,.wmv,.webm")]
    Video
}