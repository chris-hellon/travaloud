using Travaloud.Application.Catalog.TravelGuides.Specification;
using Travaloud.Domain.Catalog.TravelGuides;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.TravelGuides.Commands;

public class UpdateTravelGuideRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; } 
    public string Title { get; set; } = default!;
    public string SubTitle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public bool DeleteCurrentImage { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public FileUploadRequest? Image { get; set; }
    public IList<TravelGuideGalleryImageRequest>? TravelGuideGalleryImages { get; set; }
}

public class UpdateTravelGuideRequestHandler : IRequestHandler<UpdateTravelGuideRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TravelGuide> _repository;
    private readonly IFileStorageService _file;
    private readonly ICurrentUser _currentUser;
    
    public UpdateTravelGuideRequestHandler(IRepositoryFactory<TravelGuide> repository, IFileStorageService file, ICurrentUser currentUser)
    {
        _repository = repository;
        _file = file;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(UpdateTravelGuideRequest request, CancellationToken cancellationToken)
    {
        var travelGuide = await _repository.SingleOrDefaultAsync(new TravelGuideByIdSpec(request.Id), cancellationToken);

        if (travelGuide == null)
        {
            throw new NotFoundException("travelGuide.notfound");
        }
        
        if (request.DeleteCurrentImage)
        {
            var currentProductImagePath = travelGuide.ImagePath;
            if (!string.IsNullOrEmpty(currentProductImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductImagePath));
            }

            travelGuide = travelGuide.ClearImagePath();
        }
        
        var imagePath = request.Image is not null
            ? await _file.UploadAsync<TravelGuide>(request.Image, FileType.Image, cancellationToken)
            : null;

        var updatedTravelGuide = travelGuide.Update(request.Title,
            request.SubTitle,
            request.Description,
            request.ShortDescription,
            imagePath,
            request.MetaKeywords,
            request.MetaDescription,
            request.Title.UrlFriendly());

        await updatedTravelGuide.ProcessImages(request.TravelGuideGalleryImages, _currentUser.GetUserId(), _file, cancellationToken);
        
        updatedTravelGuide.DomainEvents.Add(EntityCreatedEvent.WithEntity(updatedTravelGuide));
        
        await _repository.UpdateAsync(updatedTravelGuide, cancellationToken);

        return updatedTravelGuide.Id;
    }
}