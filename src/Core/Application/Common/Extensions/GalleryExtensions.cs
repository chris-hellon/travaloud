using Travaloud.Application.Catalog.Galleries.Commands;
using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Common.Extensions;

public static class GalleryExtensions
{
    public static async Task ProcessImages(this Gallery gallery, IList<GalleryImageRequest>? request, DefaultIdType userId, IFileStorageService file, CancellationToken cancellationToken)
    {
        if (request?.Any() == true)
        {
            var requestImages = new List<GalleryImage>();

            foreach (var requestImage in request)
            {
                var image = gallery.GalleryImages?.FirstOrDefault(x => x.Id == requestImage.Id);

                if (image == null)
                {
                    var imagePath = await file.UploadAsync<GalleryImage>(requestImage.Image, FileType.Image, cancellationToken);
                    requestImages.Add(new GalleryImage(gallery.Title, gallery.Title, imagePath, requestImage.SortOrder, gallery.Id));
                }
                else
                {
                    image.Update(image.Title, image.Description, image.ImagePath, requestImage.SortOrder);
                    requestImages.Add(image);
                }
            }

            var imagesToRemove = gallery.GalleryImages?
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

            gallery.GalleryImages = requestImages;
        }
        else
            gallery.GalleryImages = Array.Empty<GalleryImage>();
    }
}