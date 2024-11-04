using Travaloud.Application.Catalog.Pages.Commands;
using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Queries;
using Travaloud.Application.Catalog.Seo;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IPagesService : ITransientService
{
    Task<PaginationResponse<PageDto>> SearchAsync(SearchPagesRequest request);

    Task<PageDetailsDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreatePageRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePageRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);

    Task UpdateSeoAsync(UpdateSeoRequest request);

    Task<SeoDetailsDto?> GetSeoAsync(GetSeoRequest request);

    Task<PaginationResponse<SeoRedirectDto>> GetSeoRedirects(GetSeoRedirectsRequest request);

    Task CreateSeoRedirectAsync(CreateSeoRedirectRequest request);

    Task UpdateSeoRedirectAsync(UpdateSeoRedirectRequest request);

    Task DeleteSeoRedirectAsync(DeleteSeoRedirectRequest request);

    Task<SeoRedirectDto> GetSeoRedirect(GetSeoRedirectRequest request);
}