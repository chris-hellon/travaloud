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

        var updatedPage = page.Update(request.Title, request.MetaKeywords, request.MetaDescription, request.MetaImageUrl);
        
        updatedPage.DomainEvents.Add(EntityUpdatedEvent.WithEntity(page));

        await _repository.UpdateAsync(updatedPage, cancellationToken);

        return updatedPage.Id;
    }
}