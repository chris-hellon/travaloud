using Travaloud.Application.Catalog.Partners.Dto;
using Travaloud.Application.Catalog.Partners.Specification;
using Travaloud.Domain.Catalog.Partners;

namespace Travaloud.Application.Catalog.Partners.Queries;

public class GetPartnerRequest : IRequest<PartnerDetailsDto>
{
    public GetPartnerRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

public class GetPartnerRequestHandler : IRequestHandler<GetPartnerRequest, PartnerDetailsDto>
{
    private readonly IRepositoryFactory<Partner> _repository;
    private readonly IStringLocalizer<GetPartnerRequestHandler> _localizer;

    public GetPartnerRequestHandler(IRepositoryFactory<Partner> repository,
        IStringLocalizer<GetPartnerRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<PartnerDetailsDto> Handle(GetPartnerRequest request, CancellationToken cancellationToken) =>
        await _repository.FirstOrDefaultAsync(
            new PartnerByIdSpec(request.Id), cancellationToken)
        ?? throw new NotFoundException(string.Format(_localizer["partner.notfound"], request.Id));
}