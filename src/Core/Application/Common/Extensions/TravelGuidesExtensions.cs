using Travaloud.Application.Catalog.TravelGuides.Commands;
using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Common.Extensions;

public static class TravelGuidesExtensions
{
    public static async Task ProcessImages(this TravelGuide travelGuide, IList<TravelGuideGalleryImageRequest>? request, DefaultIdType userId, IFileStorageService file, CancellationToken cancellationToken)
    {
        if (request?.Any() == true)
        {
            var requestImages = new List<TravelGuideGalleryImage>();

            foreach (var requestImage in request)
            {
                var image = travelGuide.TravelGuideGalleryImages?.FirstOrDefault(x => x.Id == requestImage.Id);

                if (image == null)
                {
                    var imagePath = await file.UploadAsync<TravelGuideGalleryImage>(requestImage.Image, FileType.Image, cancellationToken);
                    requestImages.Add(new TravelGuideGalleryImage(travelGuide.Title, travelGuide.Title, imagePath, requestImage.SortOrder, travelGuide.Id));
                }
                else
                {
                    requestImages.Add(image);
                }
            }

            var imagesToRemove = travelGuide.TravelGuideGalleryImages?
                .Where(existingImage => requestImages.All(newImage => newImage.Id != existingImage.Id))
                .ToList();

            if (imagesToRemove != null && imagesToRemove.Count != 0)
            {
                foreach (var image in imagesToRemove)
                {
                    image.DomainEvents.Add(EntityDeletedEvent.WithEntity(image));
                    image.FlagAsDeleted(userId);
                    requestImages.Add(image);
                }
            }

            travelGuide.TravelGuideGalleryImages = requestImages;
        }
        else
            travelGuide.TravelGuideGalleryImages = Array.Empty<TravelGuideGalleryImage>();
    }
}