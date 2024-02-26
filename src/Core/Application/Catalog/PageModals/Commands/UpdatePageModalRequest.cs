using MudBlazor;
using Travaloud.Application.Catalog.Extensions;
using Travaloud.Application.Catalog.PageModals.Specification;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.PageModals.Commands;

public class UpdatePageModalRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? CallToAction { get; set; }
    public DateRange? StartEndDateRange { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public FileUploadRequest? Image { get; set; }
    public IEnumerable<PageModalPageRequest>? SelectedPages { get; set; }
    public PageModalPageRequest? ValidationSelectedPages => SelectedPages?.FirstOrDefault();
}

internal class UpdatePageModalRequestHandler : IRequestHandler<UpdatePageModalRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<PageModal> _repository;
    private readonly IRepositoryFactory<PageModalLookup> _lookupRepository;
    private readonly IStringLocalizer<UpdatePageModalRequestHandler> _localizer;
    private readonly IFileStorageService _file;

    public UpdatePageModalRequestHandler(IRepositoryFactory<PageModal> repository,
        IStringLocalizer<UpdatePageModalRequestHandler> localizer,
        IFileStorageService file, IRepositoryFactory<PageModalLookup> lookupRepository)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _lookupRepository = lookupRepository;
    }

    public async Task<DefaultIdType> Handle(UpdatePageModalRequest request, CancellationToken cancellationToken)
    {
        var pageModal = await _repository.SingleOrDefaultAsync(new PageModalByIdSpec(request.Id), cancellationToken) ??
                        throw new NotFoundException(string.Format(_localizer["pageModal.notfound"], request.Id));

        // Remove old image if flag is set
        if (request.DeleteCurrentImage)
        {
            var currentProductImagePath = pageModal.ImagePath;
            if (!string.IsNullOrEmpty(currentProductImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductImagePath));
            }

            pageModal = pageModal.ClearImagePath();
        }

        var imagePath = request.Image is not null
            ? await _file.UploadAsync<PageModal>(request.Image, FileType.Image, cancellationToken)
            : null;

        await pageModal.ProcessPageModalLookups(_lookupRepository, request.SelectedPages);
        
        var updatedPageModal = pageModal.Update(request.Title, request.Description, imagePath, request.CallToAction, request.StartEndDateRange?.Start, request.StartEndDateRange?.End);
        
        pageModal.DomainEvents.Add(EntityUpdatedEvent.WithEntity(pageModal));

        await _repository.UpdateAsync(updatedPageModal, cancellationToken);

        return request.Id;
    }
}