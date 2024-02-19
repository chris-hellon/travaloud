using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Pages.Commands;
using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class PagesService : BaseService, IPagesService
{
    public PagesService(ISender mediator) : base(mediator)
    {
    }
    
    public Task<PaginationResponse<PageDto>> SearchAsync(SearchPagesRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<PageDetailsDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetPageRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreatePageRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePageRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeletePageRequest(id));
    }
}