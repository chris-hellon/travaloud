using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourDatesRequest : PaginationFilter, IRequest<PaginationResponse<TourDateDto>>
{
    public GetTourDatesRequest(DefaultIdType tourId, int requestedSpaces = 1)
    {
        TourId = tourId;
        RequestedSpaces = requestedSpaces;
    }

    public DefaultIdType TourId { get; set; }

    public int RequestedSpaces { get; set; }
}

public class GetTourDatesRequestHandler : IRequestHandler<GetTourDatesRequest, PaginationResponse<TourDateDto>>
{
    private readonly IStringLocalizer<GetTourDatesRequestHandler> _localizer;
    private readonly IRepositoryFactory<TourDate> _repository;

    public GetTourDatesRequestHandler(IRepositoryFactory<TourDate> repository,
        IStringLocalizer<GetTourDatesRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<PaginationResponse<TourDateDto>> Handle(GetTourDatesRequest request, CancellationToken cancellationToken)
    {
        var spec = new TourDatesByTourIdSpec(request);

        return await _repository.PaginatedListAsync(spec, 1, 99999, cancellationToken: cancellationToken);
    }
}