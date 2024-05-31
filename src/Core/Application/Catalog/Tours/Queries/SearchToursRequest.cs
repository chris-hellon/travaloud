using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class SearchToursRequest : PaginationFilter, IRequest<PaginationResponse<TourDto>>
{
    public string? Name { get; set; }
    public DefaultIdType? TourCategoryId { get; set; }
}

public class SearchToursRequestHandler : IRequestHandler<SearchToursRequest, PaginationResponse<TourDto>>
{
    private readonly IRepositoryFactory<Tour> _repository;

    public SearchToursRequestHandler(IRepositoryFactory<Tour> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<TourDto>> Handle(SearchToursRequest request, CancellationToken cancellationToken)
    {
        var spec = new ToursBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}