using Travaloud.Application.Catalog.Tours.Specification;
using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class UpdateTourCategoryRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ShortDescription { get; set; }
    public string? ImagePath { get; set; }
    public string? ThumbnailImagePath { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public bool? TopLevelCategory { get; set; }
    public FileUploadRequest? Image { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public DefaultIdType? ParentTourCategoryId { get; set; }
}

public class UpdateTourCategoryRequestHandler : IRequestHandler<UpdateTourCategoryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TourCategory> _repository;
    private readonly IStringLocalizer<UpdateTourCategoryRequestHandler> _localizer;
    private readonly IFileStorageService _file;

    public UpdateTourCategoryRequestHandler(IRepositoryFactory<TourCategory> repository,
        IStringLocalizer<UpdateTourCategoryRequestHandler> localizer,
        IFileStorageService file)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
    }

    public async Task<DefaultIdType> Handle(UpdateTourCategoryRequest request, CancellationToken cancellationToken)
    {
        var tourCategory = await _repository.SingleOrDefaultAsync(new TourCategoryByIdSpec(request.Id), cancellationToken);

        _ = tourCategory ?? throw new NotFoundException(string.Format(_localizer["tourCategory.notfound"], request.Id));

        // Remove old image if flag is set
        if (request.DeleteCurrentImage)
        {
            var currentProductImagePath = tourCategory.ImagePath;
            if (!string.IsNullOrEmpty(currentProductImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductImagePath));
            }

            tourCategory = tourCategory.ClearImagePath();
        }

        var imagePath = request.Image is not null
            ? await _file.UploadAsync<TourCategory>(request.Image, FileType.Image, cancellationToken)
            : null;

        var updatedTourCategory = tourCategory.Update(request.Name, request.Description, request.IconClass, request.ShortDescription, imagePath, imagePath, request.MetaKeywords, request.MetaDescription, request.TopLevelCategory, request.ParentTourCategoryId);

        // Add Domain Events to be raised after the commit
        updatedTourCategory.DomainEvents.Add(EntityUpdatedEvent.WithEntity(updatedTourCategory));

        await _repository.UpdateAsync(updatedTourCategory, cancellationToken);

        return request.Id;
    }

}