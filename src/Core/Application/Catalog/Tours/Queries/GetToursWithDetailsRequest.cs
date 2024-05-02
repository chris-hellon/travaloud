using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetToursWithDetailsRequest : IRequest<IEnumerable<TourWithoutDatesDto>>
{
    public IEnumerable<DefaultIdType> TourIds { get; set; }

    public GetToursWithDetailsRequest(IEnumerable<DefaultIdType> tourIds)
    {
        TourIds = tourIds;
    }
}

internal class GetToursWithDetailsRequestHandler : IRequestHandler<GetToursWithDetailsRequest, IEnumerable<TourWithoutDatesDto>>
{
    private readonly IRepositoryFactory<Tour> _repository;

    public GetToursWithDetailsRequestHandler(IRepositoryFactory<Tour> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourWithoutDatesDto>> Handle(GetToursWithDetailsRequest request, CancellationToken cancellationToken)
    {
        return await _repository.ListAsync(new ToursByIdsWithDetailsSpec(request), cancellationToken);
    }
}