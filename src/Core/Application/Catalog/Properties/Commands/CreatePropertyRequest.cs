using System.ComponentModel.DataAnnotations;
using Travaloud.Domain.Catalog.Pages;
using Travaloud.Domain.Catalog.Properties;
using Travaloud.Domain.Common.Events;

namespace Travaloud.Application.Catalog.Properties.Commands;

public class CreatePropertyRequest : IRequest<DefaultIdType>
{
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
    public bool? PublishToSite { get; private set; }
    public FileUploadRequest? Image { get; set; }
    public FileUploadRequest? Video { get; set; }
    public FileUploadRequest? MobileVideo { get; set; }

    [Display(Name = "Url Slug")]
    public string? UrlSlug { get; set; }

    [Display(Name = "H1 Tag")]
    public string? H1 { get; set; }

    [Display(Name = "H2 Tag")]
    public string? H2 { get; set; }

    public IList<CreatePropertyDestinationLookupRequest>? PropertyDestinationLookups { get; set; }
    public IList<CreatePropertyDirectionRequest>? Directions { get; set; }
    public IList<CreatePropertyRoomRequest>? Rooms { get; set; }
    public IList<CreatePropertyFacilityRequest>? Facilities { get; set; }
    public IList<CreatePropertyImageRequest>? Images { get; set; }
}

public class CreatePropertyRequestHandler : IRequestHandler<CreatePropertyRequest, DefaultIdType>
{
    private readonly IRepositoryFactory<Property> _repository;
    private readonly IRepositoryFactory<Page> _pageRepository;
    private readonly IFileStorageService _file;

    public CreatePropertyRequestHandler(IRepositoryFactory<Property> repository, IFileStorageService file, IRepositoryFactory<Page> pageRepository)
    {
        _repository = repository;
        _file = file;
        _pageRepository = pageRepository;
    }

    public async Task<DefaultIdType> Handle(CreatePropertyRequest request, CancellationToken cancellationToken)
    {
        var productImagePath = await _file.UploadAsync<Property>(request.Image, FileType.Image, cancellationToken);
        var videoPath = request.Video != null ? await _file.UploadAsync<Property>(request.Video, FileType.Video, cancellationToken) : null;
        var mobileVideoPath = request.MobileVideo != null ? await _file.UploadAsync<Property>(request.MobileVideo, FileType.Video, cancellationToken) : null;

        var property = new Property(request.Name,
            request.Description,
            request.ShortDescription,
            productImagePath,
            productImagePath,
            request.AddressLine1,
            request.AddressLine2,
            request.TelephoneNumber,
            request.GoogleMapsPlaceId,
            request.PageTitle,
            request.PageSubTitle,
            request.CloudbedsKey,
            request.MetaKeywords,
            request.MetaDescription,
            request.EmailAddress,
            request.PublishToSite,
            request.UrlSlug,
            request.H1,
            request.H2,
            videoPath,
            mobileVideoPath);

        property.AddDirections(request);
        property.AddFacilities(request);
        property.AddDestinations(request);
        
        await property.AddRooms(request, _file, cancellationToken);
        await property.AddImages(request, _file, cancellationToken);

        // Add Domain Events to be raised after the commit
        property.DomainEvents.Add(EntityCreatedEvent.WithEntity(property));

        await _repository.AddAsync(property, cancellationToken);
        
        var page = new Page($"Hostels - {request.Name}", property.MetaKeywords, property.MetaDescription, productImagePath);
        
        page.DomainEvents.Add(EntityCreatedEvent.WithEntity(page));
        
        await _pageRepository.AddAsync(page, cancellationToken);

        return property.Id;
    }
}