using Travaloud.Application.Catalog.Galleries.Dto;
using Travaloud.Application.Catalog.Galleries.Specification;
using Travaloud.Domain.Catalog.Galleries;

namespace Travaloud.Application.Catalog.Galleries.Queries;

public class SearchGalleriesRequest : PaginationFilter, IRequest<PaginationResponse<GalleryDto>>
{
    public string? Title { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
}

public class SearchGalleriesRequestHandler : IRequestHandler<SearchGalleriesRequest, PaginationResponse<GalleryDto>>
{
    private readonly IRepositoryFactory<Gallery> _repository;

    public SearchGalleriesRequestHandler(IRepositoryFactory<Gallery> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<GalleryDto>> Handle(SearchGalleriesRequest request, CancellationToken cancellationToken)
    {
        var spec = new GalleriesBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}