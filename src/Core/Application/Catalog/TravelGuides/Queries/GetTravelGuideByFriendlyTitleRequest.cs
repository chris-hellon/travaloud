using Travaloud.Application.Catalog.TravelGuides.Dto;
using Travaloud.Application.Catalog.TravelGuides.Specification;
using Travaloud.Domain.Catalog.TravelGuides;

namespace Travaloud.Application.Catalog.TravelGuides.Queries;

public class GetTravelGuideByFriendlyTitleRequest : IRequest<TravelGuideDetailsDto>
{
    public GetTravelGuideByFriendlyTitleRequest(string title)
    {
        Title = title;
    }

    public string Title { get; set; }
}

public class GetTravelGuideByFriendlyTitleRequestHandler : IRequestHandler<GetTravelGuideByFriendlyTitleRequest, TravelGuideDetailsDto>
{
    private readonly IRepositoryFactory<TravelGuide> _repository;
    private readonly IStringLocalizer<GetTravelGuideByFriendlyTitleRequestHandler> _localizer;

    public GetTravelGuideByFriendlyTitleRequestHandler(IRepositoryFactory<TravelGuide> repository,
        IStringLocalizer<GetTravelGuideByFriendlyTitleRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }
    
    public async Task<TravelGuideDetailsDto> Handle(GetTravelGuideByFriendlyTitleRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new TravelGuideByFriendlyTitleSpec(request.Title), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["travelGuide.notfound"], request.Title));
}