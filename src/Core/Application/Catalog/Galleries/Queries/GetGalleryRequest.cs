using Travaloud.Application.Catalog.Galleries.Dto;
using Travaloud.Application.Catalog.Galleries.Specification;
using Travaloud.Domain.Catalog.Galleries;

namespace Travaloud.Application.Catalog.Galleries.Queries;

public class GetGalleryRequest : IRequest<GalleryDetailsDto>
{
    public GetGalleryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetGalleryRequestHandler : IRequestHandler<GetGalleryRequest, GalleryDetailsDto>
{
    private readonly IRepositoryFactory<Gallery> _repository;
    private readonly IStringLocalizer<GetGalleryRequestHandler> _localizer;

    public GetGalleryRequestHandler(IRepositoryFactory<Gallery> repository,
        IStringLocalizer<GetGalleryRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<GalleryDetailsDto> Handle(GetGalleryRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new GalleryByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["gallery.notfound"], request.Id));
}