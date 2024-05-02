using Travaloud.Application.Catalog.Properties.Dto;
using Travaloud.Application.Catalog.Tours.Dto;

namespace Travaloud.Application.Catalog.Properties.Queries;

public class GetPropertyWithToursRequest : IRequest<PropertyDetailsDto?>
{
    public DefaultIdType PropertyId { get; set; }

    public GetPropertyWithToursRequest(DefaultIdType propertyId)
    {
        PropertyId = propertyId;
    }
}

internal class GetPropertyWithToursRequestHandler : IRequestHandler<GetPropertyWithToursRequest, PropertyDetailsDto?>
{
    private readonly IDapperRepository _repository;

    public GetPropertyWithToursRequestHandler(IDapperRepository repository)
    {
        _repository = repository;
    }

    public async Task<PropertyDetailsDto?> Handle(GetPropertyWithToursRequest request, CancellationToken cancellationToken)
    {
        var websiteData = await _repository.GetMultipleAsync("GetPropertyWithDetails", new { PropertyId = request.PropertyId },
            x => x.Read<PropertyDto>().ToList(),
            x => x.Read<PropertyRoomDto>().ToList(),
            x => x.Read<PropertyFacilityDto>().ToList(),
            x => x.Read<PropertyDirectionDto>().ToList(),
            x => x.Read<PropertyDirectionContentDto>().ToList(),
            x => x.Read<TourDto>().ToList(),
            x => x.Read<PropertyImageDto>().ToList());

        if (websiteData.Item1.Any())
        {
            var propertyDetailsDto = websiteData.Item1.First().Adapt<PropertyDetailsDto>();
            propertyDetailsDto.Rooms = websiteData.Item2.ToList();
            propertyDetailsDto.Facilities = websiteData.Item3.ToList();
            propertyDetailsDto.Directions = websiteData.Item4.ToList();
            propertyDetailsDto.Directions = propertyDetailsDto.Directions.Select(x =>
            {
                x.Content = websiteData.Item5.Where(dc => dc.PropertyDirectionId == x.Id).ToList();
                return x;
            }).ToList();

            propertyDetailsDto.Images = websiteData.Item7.ToList();
            propertyDetailsDto.Tours = websiteData.Item6.ToList();
            propertyDetailsDto.PageTitle ??= propertyDetailsDto.Name;
            propertyDetailsDto.PageSubTitle ??= "";

            return propertyDetailsDto;
        }

        return null;
    }
}