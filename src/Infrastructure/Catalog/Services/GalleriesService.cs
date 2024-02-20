using MediatR;
using Travaloud.Application.Catalog.Galleries.Commands;
using Travaloud.Application.Catalog.Galleries.Dto;
using Travaloud.Application.Catalog.Galleries.Queries;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class GalleriesService : BaseService, IGalleriesService
{
    public GalleriesService(ISender mediator) : base(mediator)
    {
    }

    public Task<PaginationResponse<GalleryDto>> SearchAsync(SearchGalleriesRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<GalleryDetailsDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetGalleryRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreateGalleryRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdateGalleryRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeleteGalleryRequest(id));
    }
}