using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Pages.Commands;

public class UpdatePageRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Title { get; set; } = default!;
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaImageUrl { get; set; }
    public string? CustomSeoScripts { get; set; }
    public string? UrlSlug { get; set; }
    public string? H1 { get; set; }
    public string? H2 { get; set; }
    public string? H3 { get; set; }
    public string? SeoPageTitle { get; set; }
    public List<DefaultIdType>? SelectedModals { get; set; }
}

internal class UpdatePageRequestHandler : IRequestHandler<UpdatePageRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Page> _repository;
    private readonly IStringLocalizer<UpdatePageRequestHandler> _localizer;

    public UpdatePageRequestHandler(IRepositoryFactory<Page> repository,
        IStringLocalizer<UpdatePageRequestHandler> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<DefaultIdType> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
    {
        var page = await _repository.GetByIdAsync(request.Id, cancellationToken) ??
                   throw new NotFoundException(string.Format(_localizer["page.notfound"], request.Id));

        var updatedPage = page.Update(request.Title,
            request.MetaKeywords,
            request.MetaDescription,
            request.MetaImageUrl,
            request.CustomSeoScripts,
            request.UrlSlug,
            request.H1,
            request.H2,
            request.H3,
            request.SeoPageTitle);
        
        updatedPage.DomainEvents.Add(EntityUpdatedEvent.WithEntity(page));

        await _repository.UpdateAsync(updatedPage, cancellationToken);

        return updatedPage.Id;
    }
}