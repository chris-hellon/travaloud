using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Travaloud.Infrastructure.FileStorage;

namespace Travaloud.Infrastructure.Common.Services;

public static class FileUploadHelper
{
    public static async Task<FileUploadDetails?> UploadFile(InputFileChangeEventArgs e, ISnackbar snackbar, bool isImage = true)
    {
        var extension = Path.GetExtension(e.File.Name);

        if (isImage)
        {
            if (!ApplicationConstants.SupportedImageFormats.Contains(extension.ToLower()))
            {
                snackbar.Add("Image Format Not Supported.", Severity.Error);
                return null;
            }

            if (e.File.Size > ApplicationConstants.MaxAllowedImageSize)
            {
                snackbar.Add("Image File too large, please select a file less than 1mb.", Severity.Error);
                return null;
            }
        }
        else
        {
            if (!ApplicationConstants.SupportedVideoFormats.Contains(extension.ToLower()))
            {
                snackbar.Add("Video Format Not Supported.", Severity.Error);
                return null;
            }

            if (e.File.Size > ApplicationConstants.MaxAllowedVideoSize)
            {
                snackbar.Add("Video File too large, please select a file less than 10mb.", Severity.Error);
                return null;
            }
        }

        var fileUploadDetails = new FileUploadDetails(extension);

        await using var stream = e.File.OpenReadStream(isImage ? ApplicationConstants.MaxAllowedImageSize : ApplicationConstants.MaxAllowedVideoSize);
        await using var memoryStream = new MemoryStream();
        
        await stream.CopyToAsync(memoryStream);

        var buffer = memoryStream.ToArray();
                    
        var base64 = Convert.ToBase64String(buffer);
                    
        fileUploadDetails.FileInBytes = $"data:{(isImage ? ApplicationConstants.StandardImageFormat : ApplicationConstants.StandardVideoFormat)};base64,{base64}";

        return fileUploadDetails;
    }
}