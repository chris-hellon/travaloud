using Travaloud.Domain.Catalog.Galleries;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Galleries.Commands;

public class CreateGalleryRequest : IRequest<DefaultIdType>
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public IList<GalleryImageRequest>? GalleryImages { get; set; }
}

public class CreateGalleryRequestHandler : IRequestHandler<CreateGalleryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Gallery> _repository;
    private readonly IFileStorageService _file;
    private readonly ICurrentUser _currentUser;

    public CreateGalleryRequestHandler(IRepositoryFactory<Gallery> repository, IFileStorageService file, ICurrentUser currentUser)
    {
        _repository = repository;
        _file = file;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(CreateGalleryRequest request, CancellationToken cancellationToken)
    {
        var gallery = new Gallery(request.Title, request.Description, request.MetaKeywords, request.MetaDescription);

        await gallery.ProcessImages(request.GalleryImages, _currentUser.GetUserId(), _file, cancellationToken);
        
        gallery.DomainEvents.Add(EntityCreatedEvent.WithEntity(gallery));
        
        await _repository.AddAsync(gallery, cancellationToken);

        return gallery.Id;
    }
}