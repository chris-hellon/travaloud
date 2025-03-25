using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Domain.Catalog.Pages;

namespace Travaloud.Application.Catalog.Pages.Queries;

public class GetPageByTitleRequest : IRequest<PageDetailsDto?>
{
    public string Title { get; set; }

    public GetPageByTitleRequest(string title)
    {
        Title = title;
    }
}

internal class GetPageByTitleRequestHandler : IRequestHandler<GetPageByTitleRequest, PageDetailsDto?>
{
    private readonly IRepositoryFactory<Page> _pageRepository;

    public GetPageByTitleRequestHandler(IRepositoryFactory<Page> pageRepository)
    {
        _pageRepository = pageRepository;
    }

    public async Task<PageDetailsDto?> Handle(GetPageByTitleRequest request, CancellationToken cancellationToken)
    {
        var page = await _pageRepository.SingleOrDefaultAsync(new PageByTitleSpec(request.Title), cancellationToken);

        return page.Adapt<PageDetailsDto>();
    }
}