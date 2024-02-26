using System.ComponentModel.DataAnnotations;
using Travaloud.Application.Catalog.Pages.Specification;
using Travaloud.Application.Catalog.Properties.Specification;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Properties.Commands;

public class UpdatePropertyRequest : IRequest<DefaultIdType>
{
    public DefaultIdType Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string? ShortDescription { get; set; }
    public string AddressLine1 { get; set; } = default!;
    public string? AddressLine2 { get; set; }
    public string TelephoneNumber { get; set; } = default!;
    public string? EmailAddress { get; set; }
    public string? GoogleMapsPlaceId { get; set; }
    public string? PageTitle { get; set; }
    public string? PageSubTitle { get; set; }
    public string? CloudbedsKey { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public bool DeleteCurrentImage { get; set; }
    public bool DeleteCurrentVideo { get; set; }
    public bool DeleteCurrentMobileVideo { get; set; }
    public bool? PublishToSite { get; set; }

    [Display(Name = "Url Slug")]
    public string? UrlSlug { get; set; }

    [Display(Name = "H1 Tag")]
    public string? H1 { get; set; }

    [Display(Name = "H2 Tag")]
    public string? H2 { get; set; }

    public FileUploadRequest? Image { get; set; }
    public FileUploadRequest? Video { get; set; }
    public FileUploadRequest? MobileVideo { get; set; }

    public IList<PropertyDestinationLookupRequest>? PropertyDestinationLookups { get; set; }
    public IList<PropertyDirectionRequest>? Directions { get; set; }
    public IList<PropertyRoomRequest>? Rooms { get; set; }
    public IList<PropertyFacilityRequest>? Facilities { get; set; }
    public IList<PropertyImageRequest>? Images { get; set; }
}

public class UpdatePropertyRequestHandler : IRequestHandler<UpdatePropertyRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Property> _repository;
    private readonly IRepositoryFactory<Page> _pageRepository;
    private readonly IStringLocalizer<UpdatePropertyRequestHandler> _localizer;
    private readonly IFileStorageService _file;
    private readonly ICurrentUser _currentUser;
    
    public UpdatePropertyRequestHandler(IRepositoryFactory<Property> repository,
        IStringLocalizer<UpdatePropertyRequestHandler> localizer,
        IFileStorageService file, IRepositoryFactory<Page> pageRepository, ICurrentUser currentUser)
    {
        _repository = repository;
        _localizer = localizer;
        _file = file;
        _pageRepository = pageRepository;
        _currentUser = currentUser;
    }

    public async Task<DefaultIdType> Handle(UpdatePropertyRequest request, CancellationToken cancellationToken)
    {
        var property = await _repository.SingleOrDefaultAsync(new PropertyByIdSpec(request.Id), cancellationToken);

        _ = property ?? throw new NotFoundException(string.Format(_localizer["property.notfound"], request.Id));

        var page = await _pageRepository.SingleOrDefaultAsync(new PageByTitleSpec($"Hostels - {property.Name}"), cancellationToken);
        
        // Remove old image if flag is set
        if (request.DeleteCurrentImage)
        {
            var currentProductImagePath = property.ImagePath;
            if (!string.IsNullOrEmpty(currentProductImagePath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductImagePath));
            }

            property = property.ClearImagePath();
        }

        if (request.DeleteCurrentVideo)
        {
            var currentProductVideoPath = property.VideoPath;
            if (!string.IsNullOrEmpty(currentProductVideoPath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductVideoPath));
            }

            property = property.ClearVideoPath();
        }

        if (request.DeleteCurrentMobileVideo)
        {
            var currentProductMobileVideoPathPath = property.MobileVideoPath;
            if (!string.IsNullOrEmpty(currentProductMobileVideoPathPath))
            {
                var root = Directory.GetCurrentDirectory();
                await _file.Remove(Path.Combine(root, currentProductMobileVideoPathPath));
            }

            property = property.ClearMobileVideoPath();
        }

        var propertyImagePath = request.Image is not null
            ? await _file.UploadAsync<Property>(request.Image, FileType.Image, cancellationToken)
            : null;

        var propertyVideoPath = request.Video is not null
            ? await _file.UploadAsync<Property>(request.Video, FileType.Video, cancellationToken)
            : null;

        var propertyMobileVideoPath = request.MobileVideo is not null
            ? await _file.UploadAsync<Property>(request.MobileVideo, FileType.Video, cancellationToken)
            : null;

        var updatedProperty = property.Update(request.Name, request.Description, request.ShortDescription, propertyImagePath, propertyImagePath, request.AddressLine1, request.AddressLine2, request.TelephoneNumber, request.GoogleMapsPlaceId, request.PageTitle, request.PageSubTitle, request.CloudbedsKey, request.MetaKeywords, request.MetaDescription, request.EmailAddress, request.PublishToSite, request.UrlSlug, request.H1, request.H2, propertyVideoPath, propertyMobileVideoPath);

        var userId = _currentUser.GetUserId();
        
        updatedProperty.ProcessDirections(request.Directions, userId);
        updatedProperty.ProcessFacilities(request.Facilities, userId);
        
        if (request.PropertyDestinationLookups != null)
            updatedProperty.ProcessDestinations(request.PropertyDestinationLookups, userId);

        await updatedProperty.ProcessRooms(request.Rooms, userId, _file, cancellationToken);

        await updatedProperty.ProcessImages(request.Images, userId, _file, cancellationToken);

        // Add Domain Events to be raised after the commit
        property.DomainEvents.Add(EntityUpdatedEvent.WithEntity(property));

        await _repository.UpdateAsync(updatedProperty, cancellationToken);
        
        if (page != null)
        {
            var updatedPage = page.Update($"Hostels - {request.Name}", request.MetaKeywords, request.MetaDescription, propertyImagePath);
            
            page.DomainEvents.Add(EntityUpdatedEvent.WithEntity(page));
            
            await _pageRepository.UpdateAsync(updatedPage, cancellationToken);

        }
        else
        {
            page = new Page($"Hostels - {request.Name}", request.MetaKeywords, request.MetaDescription, propertyImagePath);
        
            page.DomainEvents.Add(EntityCreatedEvent.WithEntity(page));
        
            await _pageRepository.AddAsync(page, cancellationToken);
        }
        
        return request.Id;
    }
}