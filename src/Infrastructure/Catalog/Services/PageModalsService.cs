using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.PageModals.Commands;
using Travaloud.Application.Catalog.PageModals.Dto;
using Travaloud.Application.Catalog.PageModals.Queries;
using Travaloud.Application.Common.Models;

namespace Travaloud.Infrastructure.Catalog.Services;

public class PageModalsService : BaseService, IPageModalsService
{
    public PageModalsService(ISender mediator) : base(mediator)
    {
    }
    
    public Task<PaginationResponse<PageModalDto>> SearchAsync(SearchPageModalsRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<PageModalDetailsDto> GetAsync(DefaultIdType id)
    {
        return Mediator.Send(new GetPageModalRequest(id));
    }
    
    public Task<DefaultIdType> CreateAsync(CreatePageModalRequest request)
    {
        return Mediator.Send(request);
    }
    
    public async Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePageModalRequest request)
    {
        if (id != request.Id)
            throw new ConflictException("Id and request.Id do not match");
        
        return await Mediator.Send(request);
    }
    
    public Task<DefaultIdType> DeleteAsync(DefaultIdType id)
    {
        return Mediator.Send(new DeletePageModalRequest(id));
    }
}