using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Specification;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Queries;

public class SearchTravelGuidesRequest : PaginationFilter, IRequest<PaginationResponse<TravelGuideDto>>
{
    public string? Title { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
}

public class SearchTravelGuidesRequestHandler : IRequestHandler<SearchTravelGuidesRequest, PaginationResponse<TravelGuideDto>>
{
    private readonly IRepositoryFactory<TravelGuide> _repository;

    public SearchTravelGuidesRequestHandler(IRepositoryFactory<TravelGuide> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<TravelGuideDto>> Handle(SearchTravelGuidesRequest request, CancellationToken cancellationToken)
    {
        var spec = new TravelGuidesBySearchSpec(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}