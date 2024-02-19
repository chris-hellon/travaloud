using Travaloud.Application.Catalog.Pages.Commands;
using Travaloud.Application.Catalog.Pages.Dto;
using Travaloud.Application.Catalog.Pages.Queries;

namespace Travaloud.Application.Catalog.Interfaces;

public interface IPagesService : ITransientService
{
    Task<PaginationResponse<PageDto>> SearchAsync(SearchPagesRequest request);

    Task<PageDetailsDto> GetAsync(DefaultIdType id);

    Task<DefaultIdType> CreateAsync(CreatePageRequest request);

    Task<DefaultIdType?> UpdateAsync(DefaultIdType id, UpdatePageRequest request);

    Task<DefaultIdType> DeleteAsync(DefaultIdType id);
}