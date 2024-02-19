using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.PageModals.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.PageModals.Queries;

public class GetPageModalRequest : IRequest<PageModalDetailsDto>
{
    public DefaultIdType Id { get; set; }

    public GetPageModalRequest(DefaultIdType id) => Id = id;
}

internal class GetPageModalRequestHandler : IRequestHandler<GetPageModalRequest, PageModalDetailsDto>
{
    private readonly IRepositoryFactory<PageModal> _repository;
    private readonly IStringLocalizer<GetPageModalRequestHandler> _localizer;

    public GetPageModalRequestHandler(IRepositoryFactory<PageModal> repository,
        IStringLocalizer<GetPageModalRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<PageModalDetailsDto> Handle(GetPageModalRequest request, CancellationToken cancellationToken)
    {
        var page = await _repository.SingleOrDefaultAsync(new PageModalByIdSpec(request.Id), cancellationToken) ??
                   throw new NotFoundException(string.Format(_localizer["page.notfound"], request.Id));

        var pageDetailsDto = page.Adapt<PageModalDetailsDto>();

        var modals = page.PageModalLookups;
        if (modals != null)
            pageDetailsDto.PageModalLookups = modals.Select(x => new PageModalLookupDto()
            {
                Id = x.Id,
                PageId = x.PageId,
                PageModalId = x.PageModalId,
                Title = x.Page.Title
            });

        return pageDetailsDto;
    }
}