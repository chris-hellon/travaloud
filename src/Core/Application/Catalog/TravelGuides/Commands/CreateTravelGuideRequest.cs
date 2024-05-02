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
    private readonly ICurrentUser _currentUser;

    public CreateTravelGuideRequestHandler(IRepositoryFactory<TravelGuide> repository, IFileStorageService file, ICurrentUser currentUser)
    {
        _repository = repository;
        _file = file;
        _currentUser = currentUser;
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
            request.MetaDescription,
            request.Title.UrlFriendly()
        );

        await travelGuide.ProcessImages(request.TravelGuideGalleryImages, _currentUser.GetUserId(), _file, cancellationToken);
        
        travelGuide.DomainEvents.Add(EntityCreatedEvent.WithEntity(travelGuide));
        
        await _repository.AddAsync(travelGuide, cancellationToken);

        return travelGuide.Id;
    }
}