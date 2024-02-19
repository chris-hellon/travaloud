using MudBlazor;
using Travaloud.Application.Catalog.Extensions;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.PageModals.Commands;

public class CreatePageModalRequest : IRequest<DefaultIdType>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? CallToAction { get; set; }
    public DateRange? StartEndDateRange { get; set; }
    public FileUploadRequest? Image { get; set; }
    public IEnumerable<PageModalPageRequest>? SelectedPages { get; set; }
    public PageModalPageRequest? _validationSelectedPages => SelectedPages?.FirstOrDefault();
}

internal class CreatePageModalRequestHandler : IRequestHandler<CreatePageModalRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<PageModal> _repository;
    private readonly IRepositoryFactory<PageModalLookup> _lookupRepository;
    private readonly IFileStorageService _file;

    public CreatePageModalRequestHandler(IRepositoryFactory<PageModal> repository, IFileStorageService file, IRepositoryFactory<PageModalLookup> lookupRepository)
    {
        _repository = repository;
        _file = file;
        _lookupRepository = lookupRepository;
    }

    public async Task<DefaultIdType> Handle(CreatePageModalRequest request, CancellationToken cancellationToken)
    {
        var imagePath = await _file.UploadAsync<PageModal>(request.Image, FileType.Image, cancellationToken);
        
        var pageModal = new PageModal(request.Title, request.Description, imagePath, request.CallToAction, request.StartEndDateRange?.Start, request.StartEndDateRange?.End);

        // add page lookups
        await pageModal.ProcessPageModalLookups(_lookupRepository, request.SelectedPages);

        // add domain events
        pageModal.DomainEvents.Add(EntityCreatedEvent.WithEntity(pageModal));

        // add entity
        await _repository.AddAsync(pageModal, cancellationToken);

        return pageModal.Id;
    }
}