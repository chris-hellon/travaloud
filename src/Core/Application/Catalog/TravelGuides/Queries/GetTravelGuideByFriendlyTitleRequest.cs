using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Specification;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Queries;

public class GetTravelGuideByFriendlyTitleRequest : IRequest<TravelGuideDto>
{
    public GetTravelGuideByFriendlyTitleRequest(string title)
    {
        Title = title;
    }

    public string Title { get; set; }
}

public class GetTravelGuideByFriendlyTitleRequestHandler : IRequestHandler<GetTravelGuideByFriendlyTitleRequest, TravelGuideDto>
{
    private readonly IRepositoryFactory<TravelGuide> _repository;
    private readonly IStringLocalizer<GetTravelGuideByFriendlyTitleRequestHandler> _localizer;

    public GetTravelGuideByFriendlyTitleRequestHandler(IRepositoryFactory<TravelGuide> repository,
        IStringLocalizer<GetTravelGuideByFriendlyTitleRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<TravelGuideDto> Handle(GetTravelGuideByFriendlyTitleRequest request, CancellationToken cancellationToken)
    {
        var spec = new TravelGuidesWithDetailsSpec(new SearchTravelGuidesRequest());
        var travelGuides = await _repository.PaginatedListAsync(spec, 1, 99999, cancellationToken: cancellationToken);
        var travelGuide = travelGuides.Data.FirstOrDefault(x => x.Title.UrlFriendly() == request.Title);
        
        return travelGuide != null ? travelGuide.Adapt<TravelGuideDto>() : throw new NotFoundException(string.Format(_localizer["travelGuide.notfound"], request.Title));
    }
}