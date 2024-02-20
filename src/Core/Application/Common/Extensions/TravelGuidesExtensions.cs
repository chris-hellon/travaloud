using Travaloud.Application.Catalog.TravelGuides.Commands;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Common.Extensions;

public static class TravelGuidesExtensions
{
    public static async Task AddImages(this TravelGuide property, UpdateTravelGuideRequest request, IFileStorageService file, CancellationToken cancellationToken)
    {
        if (request.TravelGuideGalleryImages?.Any() == true)
        {
            var images = new List<TravelGuideGalleryImage>();
            foreach (var imageRequest in request.TravelGuideGalleryImages)
            {
                var image = property.TravelGuideGalleryImages?.FirstOrDefault(x => x.Id == imageRequest.Id);

                if (image == null)
                {
                    var imagePath = await file.UploadAsync<TravelGuideGalleryImage>(imageRequest.Image, FileType.Image, cancellationToken);
                    images.Add(new TravelGuideGalleryImage(request.Title, request.Title, imagePath, imageRequest.SortOrder, property.Id));
                }
                else
                {
                    if (imageRequest.DeleteCurrentImage)
                    {
                        var currentProductImagePath = imageRequest.ImagePath;
                        if (!string.IsNullOrEmpty(currentProductImagePath))
                        {
                            var root = Directory.GetCurrentDirectory();
                            await file.Remove(Path.Combine(root, currentProductImagePath));
                        }

                        image = image.ClearImagePath();
                    }

                    var imagePath = imageRequest.Image is not null
                        ? image.ImagePath
                        : null;

                    image.Update(request.Title, request.Title, imagePath, imageRequest.SortOrder, property.Id);
                    images.Add(image);
                }
            }

            property.TravelGuideGalleryImages = images;
        }
    }
}