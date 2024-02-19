using Travaloud.Domain.Catalog.Tours;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Tours.Commands;

public class CreateTourCategoryRequest : IRequest<DefaultIdType>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? IconClass { get; set; }
    public string? ShortDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public bool? TopLevelCategory { get; set; }
    public DefaultIdType? ParentTourCategoryId { get; set; }
    public FileUploadRequest? Image { get; set; }
}

public class CreateTourCategoryRequestHandler : IRequestHandler<CreateTourCategoryRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<TourCategory> _repository;
    private readonly IFileStorageService _file;

    public CreateTourCategoryRequestHandler(IRepositoryFactory<TourCategory> repository, IFileStorageService file)
    {
        _repository = repository;
        _file = file;
    }

    public async Task<DefaultIdType> Handle(CreateTourCategoryRequest request, CancellationToken cancellationToken)
    {
        var imagePath = await _file.UploadAsync<Tour>(request.Image, FileType.Image, cancellationToken);

        var tourCategory = new TourCategory(request.Name, request.Description, request.IconClass, request.ShortDescription, imagePath, imagePath, request.MetaKeywords, request.MetaDescription, request.TopLevelCategory, request.ParentTourCategoryId);

        // Add Domain Events to be raised after the commit
        tourCategory.DomainEvents.Add(EntityCreatedEvent.WithEntity(tourCategory));

        await _repository.AddAsync(tourCategory, cancellationToken);

        return tourCategory.Id;
    }
}