using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Queries;

public class SearchPartnersRequest : PaginationFilter, IRequest<PaginationResponse<PartnerDto>>
{
    public string? Name { get; set; }
}

public class SearchPartnersRequestHandler : IRequestHandler<SearchPartnersRequest, PaginationResponse<PartnerDto>>
{
    private readonly IRepositoryFactory<Partner> _repository;

    public SearchPartnersRequestHandler(IRepositoryFactory<Partner> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<PartnerDto>> Handle(SearchPartnersRequest request, CancellationToken cancellationToken)
    {
        var spec = new PartnersBySearchRequest(request);
        return await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken: cancellationToken);
    }
}