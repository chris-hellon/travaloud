using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetTourItineraryRequest : IRequest<TourItineraryDto>
{
    public GetTourItineraryRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetTourItineraryRequesttHandler : IRequestHandler<GetTourItineraryRequest, TourItineraryDto>
{
    private readonly IRepositoryFactory<TourItinerary> _repository;
    private readonly IStringLocalizer<GetTourItineraryRequesttHandler> _localizer;

    public GetTourItineraryRequesttHandler(IRepositoryFactory<TourItinerary> repository,
        IStringLocalizer<GetTourItineraryRequesttHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<TourItineraryDto> Handle(GetTourItineraryRequest request, CancellationToken cancellationToken)
    {
        var tourItinerary = await _repository.FirstOrDefaultAsync(
                                new TourItineraryByIdSpec(request.Id), cancellationToken)
                            ?? throw new NotFoundException(string.Format(_localizer["tour.notfound"], request.Id));

        tourItinerary.Sections = tourItinerary.Sections.OrderBy(x => x.SortOrder ?? int.MaxValue).ToList();

        foreach (var section in tourItinerary.Sections)
        {
            section.Images = section.Images.OrderBy(x => x.SortOrder ?? int.MaxValue).ToList();
        }

        return tourItinerary;
    }
}