using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TravelGuides.Commands;

public class CreateTravelGuideRequest : IRequest<DefaultIdType>
{
    public string Title { get; set; } = default!;
    public string SubTitle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public FileUploadRequest? Image { get; set; }
    public IList<TravelGuideGalleryImageRequest>? TravelGuideGalleryImages { get; set; }
}

public class CreateTravelGuideRequestHandler : IRequestHandler<CreateTravelGuideRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TravelGuide> _repository;
    private readonly IFileStorageService _file;

    public CreateTravelGuideRequestHandler(IRepositoryFactory<TravelGuide> repository, IFileStorageService file)
    {
        _repository = repository;
        _file = file;
    }

    public async Task<DefaultIdType> Handle(CreateTravelGuideRequest request, CancellationToken cancellationToken)
    {
        var imagePath = await _file.UploadAsync<TravelGuide>(request.Image, FileType.Image, cancellationToken);

        var travelGuide = new TravelGuide(
            request.Title,
            request.SubTitle,
            request.Description,
            request.ShortDescription,
            imagePath,
            request.MetaKeywords,
            request.MetaDescription
        );

        if (request.TravelGuideGalleryImages != null && request.TravelGuideGalleryImages.Any())
        {
            var galleryImages = new List<TravelGuideGalleryImage>();
            foreach (var galleryImage in request.TravelGuideGalleryImages)
            {
                var galleryImagePath = await _file.UploadAsync<TravelGuideGalleryImage>(request.Image, FileType.Image, cancellationToken);
                
                galleryImages.Add(new TravelGuideGalleryImage(
                    request.Title,
                    request.Title,
                    galleryImagePath,
                    galleryImage.SortOrder
                    ));
            }

            travelGuide.TravelGuideGalleryImages = galleryImages;
        }
        
        travelGuide.DomainEvents.Add(EntityCreatedEvent.WithEntity(travelGuide));
        
        await _repository.AddAsync(travelGuide, cancellationToken);

        return travelGuide.Id;
    }
}