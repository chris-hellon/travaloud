namespace Travaloud.Infrastructure.FileStorage;

public class FileUploadDetails
{
    public string Extension { get; set; } = default!;
    public string FileInBytes { get; set; } = default!;

    public FileUploadDetails(string extension)
    {
        Extension = extension;
    }
}