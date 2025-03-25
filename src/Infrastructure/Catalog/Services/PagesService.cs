using MediatR;
using Travaloud.Application.Catalog.Interfaces;
using Travaloud.Application.Catalog.Pages.Commands;
using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Queries;
using Travaloud.Application.Catalog.Seo;
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

    public Task UpdateSeoAsync(UpdateSeoRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<SeoDetailsDto?> GetSeoAsync(GetSeoRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<PaginationResponse<SeoRedirectDto>> GetSeoRedirects(GetSeoRedirectsRequest request)
    {
        return Mediator.Send(request);
    }

    public Task CreateSeoRedirectAsync(CreateSeoRedirectRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task UpdateSeoRedirectAsync(UpdateSeoRedirectRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task DeleteSeoRedirectAsync(DeleteSeoRedirectRequest request)
    {
        return Mediator.Send(request);
    }
    
    public Task<SeoRedirectDto> GetSeoRedirect(GetSeoRedirectRequest request)
    {
        return Mediator.Send(request);
    }

    public Task<PageDetailsDto?> GetPageByTitle(GetPageByTitleRequest request)
    {
        return Mediator.Send(request);
    }
}