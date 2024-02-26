using Travaloud.Application.Catalog.Tours.Dto;
using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;

namespace Travaloud.Application.Catalog.Tours.Queries;

public class GetToursByPublishToWebsiteRequest : PaginationFilter, IRequest<IEnumerable<TourDto>>
{
   
}

public class GetToursByPublishToWebsiteRequestHandler : IRequestHandler<GetToursByPublishToWebsiteRequest, IEnumerable<TourDto>>
{
    private readonly IRepositoryFactory<Tour> _repository;

    public GetToursByPublishToWebsiteRequestHandler(IRepositoryFactory<Tour> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TourDto>> Handle(GetToursByPublishToWebsiteRequest request, CancellationToken cancellationToken)
    {
        var spec = new ToursByPublishToWebsiteSpec(request);

        return await _repository.ListAsync(spec, cancellationToken: cancellationToken);
    }
}