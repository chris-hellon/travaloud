using Travaloud.Application.Catalog.TourDates.Specification;
using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.TourDates.Queries;

public class GetTourDateRequest : IRequest<TourDateDto>
{
    public GetTourDateRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

internal class GetTourDateRequestHandler : IRequestHandler<GetTourDateRequest, TourDateDto>
{
    private readonly IRepositoryFactory<TourDate> _repository;
    private readonly IStringLocalizer<GetTourDateRequestHandler> _localizer;

    public GetTourDateRequestHandler(IRepositoryFactory<TourDate> repository,
        IStringLocalizer<GetTourDateRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<TourDateDto> Handle(GetTourDateRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new TourDateWithoutPriceByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["tourdate.notfound"], request.Id));
}