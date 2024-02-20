using Travaloud.Application.Catalog.Galleries.Specification;
using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Galleries.Commands;

public class UpdateGalleryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; } 
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public IList<GalleryImageRequest>? GalleryImages { get; set; }
}

public class UpdateGalleryRequestHandler : IRequestHandler<UpdateGalleryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Gallery> _repository;
    private readonly IRepositoryFactory<GalleryImage> _imageRepository;
    private readonly IFileStorageService _file;
    private readonly ICurrentUser _currentUser;

    public UpdateGalleryRequestHandler(IRepositoryFactory<Gallery> repository, IFileStorageService file, IRepositoryFactory<GalleryImage> imageRepository, ICurrentUser currentUser)
    {
        _repository = repository;
        _file = file;
        _imageRepository = imageRepository;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(UpdateGalleryRequest request, CancellationToken cancellationToken)
    {
        var gallery = await _repository.SingleOrDefaultAsync(new GalleryByIdSpec(request.Id), cancellationToken);

        if (gallery == null)
        {
            throw new NotFoundException("Gallery not found");
        }

        var updatedGallery = gallery.Update(request.Title,
            request.Description,
            request.MetaKeywords,
            request.MetaDescription);

        await updatedGallery.ProcessImages(request.GalleryImages, _currentUser.GetUserId(), _file, cancellationToken);
        
        updatedGallery.DomainEvents.Add(EntityUpdatedEvent.WithEntity(updatedGallery));
        
        await _repository.UpdateAsync(updatedGallery, cancellationToken);

        return updatedGallery.Id;
    }
}
