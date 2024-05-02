using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Specification;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Queries;

public class GetTravelGuideRequest : IRequest<TravelGuideDetailsDto>
{
    public GetTravelGuideRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetTravelGuideRequestHandler : IRequestHandler<GetTravelGuideRequest, TravelGuideDetailsDto>
{
    private readonly IRepositoryFactory<TravelGuide> _repository;
    private readonly IStringLocalizer<GetTravelGuideRequestHandler> _localizer;

    public GetTravelGuideRequestHandler(IRepositoryFactory<TravelGuide> repository,
        IStringLocalizer<GetTravelGuideRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<TravelGuideDetailsDto> Handle(GetTravelGuideRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new TravelGuideByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["travelGuide.notfound"], request.Id));
}