using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Queries;

public class GetPageRequest : IRequest<PageDetailsDto>
{
    public GetPageRequest(DefaultIdType id)
    {
        Id = id;
    }

    public DefaultIdType Id { get; set; }
}

internal class GetPageRequestHandler : IRequestHandler<GetPageRequest, PageDetailsDto>
{
    private readonly IRepositoryFactory<Page> _repository;
    private readonly IStringLocalizer<GetPageRequestHandler> _localizer;

    public GetPageRequestHandler(IRepositoryFactory<Page> repository, IStringLocalizer<GetPageRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<PageDetailsDto> Handle(GetPageRequest request, CancellationToken cancellationToken)
    {
        var page = await _repository.SingleOrDefaultAsync(new PageByIdSpec(request.Id), cancellationToken) ??
                   throw new NotFoundException(string.Format(_localizer["page.notfound"], request.Id));

        var pageDetailsDto = page.Adapt<PageDetailsDto>();

        var modals = page.PageModalLookups?.Select(x => x.PageModal);
        if (modals != null) pageDetailsDto.PageModals = modals.Adapt<IList<PageModalDto>>();

        return pageDetailsDto;
    }
}