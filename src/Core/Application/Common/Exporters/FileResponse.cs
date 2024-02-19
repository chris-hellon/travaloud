namespace Travaloud.Application.Common.Exporters;

public class FileResponse
{
    public Stream Stream { get; private set; }

    public FileResponse(Stream stream)
    {
        Stream = stream;
    }
}