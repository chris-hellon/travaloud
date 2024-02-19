using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Pages.Commands;

public class CreatePageRequest : IRequest<DefaultIdType>
{
    public string Title { get; set; } = default!;
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaImageUrl { get; set; }
    public List<DefaultIdType>? SelectedModals { get; set; }
}

internal class CreatePageRequestHandler : IRequestHandler<CreatePageRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Page> _repository;

    public CreatePageRequestHandler(IRepositoryFactory<Page> repository)
    {
        _repository = repository;
    }

    public async Task<DefaultIdType> Handle(CreatePageRequest request, CancellationToken cancellationToken)
    {
        var page = new Page(request.Title, request.MetaKeywords, request.MetaDescription, request.MetaImageUrl);

        // add domain events
        page.DomainEvents.Add(EntityCreatedEvent.WithEntity(page));

        await _repository.AddAsync(page, cancellationToken);

        return page.Id;
    }
}