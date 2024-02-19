using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Finbuckle.MultiTenant;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeMapping;
using Travaloud.Application.Common.FileStorage;
using Travaloud.Domain.Common;
using Travaloud.Infrastructure.Common.Extensions;

namespace Travaloud.Infrastructure.FileStorage;

public class AzureStorageService : IFileStorageService
{
    private readonly AzureStorageSettings _settings;
    private readonly ILogger<AzureStorageService> _logger;
    private readonly ITenantInfo _tenantInfo;

    public AzureStorageService(IOptions<AzureStorageSettings> settings, ITenantInfo tenantInfo, ILogger<AzureStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _tenantInfo = tenantInfo;
    }

    public async Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType, CancellationToken cancellationToken = default)
        where T : class
    {
        if (request?.Data == null)
        {
            return string.Empty;
        }

        if (!supportedFileType.GetDescriptionList().Contains(request.Extension.ToLower()))
            throw new InvalidOperationException("File Format Not Supported.");
        if (request.Name is null)
            throw new InvalidOperationException("Name is required.");

        var base64Data = Regex.Match(request.Data, supportedFileType == FileType.Image ? "data:image/(?<type>.+?),(?<data>.+)" : "data:video/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

        var fileBytes = Convert.FromBase64String(base64Data);
        var fileName = $"assets/images/" + DefaultIdType.NewGuid().ToString() + request.Extension;

        var container = new BlobContainerClient(_settings.ConnectionString, $"{_tenantInfo.Id}/assets/images");
        var client = container.GetBlobClient(fileName);

        using var stream = new MemoryStream(fileBytes, writable: false);

        try
        {
            var uploadResponse = await client.UploadAsync(
                stream,
                new BlobUploadOptions()
                {
                    HttpHeaders = new BlobHttpHeaders()
                    {
                        ContentType = MimeUtility.GetMimeMapping(fileName),
                        CacheControl = "31536000"
                    }
                },
                cancellationToken);

            return client.Uri.AbsoluteUri;
        }
        catch (Exception ex)
        {
            var error = ex.Message;
            return string.Empty;
        }
    }

    public async Task Remove(string? path)
    {
        if (path != null)
        {
            var container = new BlobContainerClient(_settings.ConnectionString, $"{_tenantInfo.Id}/assets/images");
            var file = container.GetBlobClient(path.Replace($"{_tenantInfo.Id}/assets/images", string.Empty));

            if (await file.ExistsAsync())
                await file.DeleteAsync();
        }
    }
}